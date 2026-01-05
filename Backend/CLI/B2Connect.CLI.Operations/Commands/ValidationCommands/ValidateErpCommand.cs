using System.CommandLine;
using B2Connect.Api.Validation;
using B2Connect.Api.Models.Erp;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace B2Connect.CLI.Operations.Commands.ValidationCommands;

public static class ValidateErpCommand
{
    public static Command Create()
    {
        var command = new Command("validate-erp", "Validate ERP data with connector-specific rules");

        var erpTypeOption = new Option<string>("--erp-type", "ERP system type (e.g., enventa, sap)") { IsRequired = true };
        var tenantIdOption = new Option<string>("--tenant-id", "Tenant identifier") { IsRequired = true };
        var dataFileOption = new Option<FileInfo>("--data-file", "JSON file with ERP data to validate");
        var outputOption = new Option<FileInfo>("--output", "Output validation report to file");
        var dataQualityReportOption = new Option<bool>("--data-quality-report", "Generate detailed data quality report with solution hints");
        var includeSqlOption = new Option<bool>("--include-sql", "Include SQL queries used to retrieve the data");

        command.AddOption(erpTypeOption);
        command.AddOption(tenantIdOption);
        command.AddOption(dataFileOption);
        command.AddOption(outputOption);
        command.AddOption(dataQualityReportOption);
        command.AddOption(includeSqlOption);

        command.SetHandler(ExecuteAsync, erpTypeOption, tenantIdOption, dataFileOption, outputOption, dataQualityReportOption, includeSqlOption);

        return command;
    }

    private static async Task ExecuteAsync(string erpType, string tenantId, FileInfo? dataFile, FileInfo? output, bool dataQualityReport, bool includeSql)
    {
        // Create validators manually (since CLI doesn't have full DI setup)
        var logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<ErpSpecificValidator>();
        var rulesProvider = new ValidationRulesProvider(); // Simple implementation

        // Create connector based on ERP type
        IErpConnector connector = erpType.ToLower() switch
        {
            "enventa" => new SimpleEnventaConnector(logger),
            "sap" => new SimpleSapConnector(logger),
            _ => throw new ArgumentException($"Unsupported ERP type: {erpType}")
        };

        var validator = new ErpSpecificValidator(logger, connector, rulesProvider);

        // Load data from file or stdin
        var rawData = await LoadErpDataAsync(dataFile);

        // Set ERP type and tenant ID on each data item
        var data = rawData.Select(item =>
        {
            item.ErpType = erpType;
            item.TenantId = tenantId;
            return item;
        }).ToList();

        // Validate
        var results = await validator.ValidateCollectionAsync(data, CancellationToken.None);

        // Output results
        if (dataQualityReport)
        {
            await WriteDataQualityReportAsync(results, data, erpType, tenantId, output, includeSql);
        }
        else
        {
            await WriteValidationReportAsync(results, output);
        }

        // Set exit code based on validation results
        Environment.ExitCode = results.Any(r => r.Severity >= B2Connect.Api.Validation.ValidationSeverity.Error) ? 1 : 0;
    }

    private static async Task<IEnumerable<ErpData>> LoadErpDataAsync(FileInfo? dataFile)
    {
        if (dataFile != null && dataFile.Exists)
        {
            using var stream = dataFile.OpenRead();
            var data = await JsonSerializer.DeserializeAsync<List<ErpData>>(stream);
            return data ?? Enumerable.Empty<ErpData>();
        }
        else
        {
            // Read from stdin
            using var reader = new StreamReader(Console.OpenStandardInput());
            var json = await reader.ReadToEndAsync();
            if (string.IsNullOrWhiteSpace(json))
            {
                return Enumerable.Empty<ErpData>();
            }
            var data = JsonSerializer.Deserialize<List<ErpData>>(json);
            return data ?? Enumerable.Empty<ErpData>();
        }
    }

    private static async Task WriteValidationReportAsync(IEnumerable<B2Connect.Api.Validation.ValidationResult> results, FileInfo? output)
    {
        var report = new
        {
            Timestamp = DateTime.UtcNow,
            TotalRecords = results.Count(),
            ValidRecords = results.Count(r => r.IsValid),
            InvalidRecords = results.Count(r => !r.IsValid),
            Errors = results.Where(r => r.Severity >= B2Connect.Api.Validation.ValidationSeverity.Error).Count(),
            Warnings = results.Where(r => r.Severity == B2Connect.Api.Validation.ValidationSeverity.Warning).Count(),
            Results = results
        };

        var json = JsonSerializer.Serialize(report, new JsonSerializerOptions { WriteIndented = true });

        if (output != null)
        {
            await File.WriteAllTextAsync(output.FullName, json);
            Console.WriteLine($"Validation report written to {output.FullName}");
        }
        else
        {
            Console.WriteLine(json);
        }
    }

    private static async Task WriteDataQualityReportAsync(
        IEnumerable<B2Connect.Api.Validation.ValidationResult> results,
        IEnumerable<ErpData> originalData,
        string erpType,
        string tenantId,
        FileInfo? output,
        bool includeSql)
    {
        var dataQualityIssues = results.SelectMany((result, index) =>
            CreateDataQualityIssues(result, originalData.ElementAtOrDefault(index), erpType, tenantId, includeSql));

        var report = new
        {
            Timestamp = DateTime.UtcNow,
            ErpType = erpType,
            TenantId = tenantId,
            Summary = new
            {
                TotalRecords = originalData.Count(),
                ValidRecords = results.Count(r => r.IsValid),
                InvalidRecords = results.Count(r => !r.IsValid),
                TotalIssues = dataQualityIssues.Count(),
                Errors = dataQualityIssues.Count(i => i.Severity >= B2Connect.Api.Validation.ValidationSeverity.Error),
                Warnings = dataQualityIssues.Count(i => i.Severity == B2Connect.Api.Validation.ValidationSeverity.Warning),
                Critical = dataQualityIssues.Count(i => i.Severity == B2Connect.Api.Validation.ValidationSeverity.Critical)
            },
            SqlQueries = includeSql ? GenerateSqlQueries(erpType, tenantId) : null,
            Issues = dataQualityIssues
        };

        var json = JsonSerializer.Serialize(report, new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        if (output != null)
        {
            await File.WriteAllTextAsync(output.FullName, json);
            Console.WriteLine($"Data quality report written to: {output.FullName}");
        }
        else
        {
            Console.WriteLine(json);
        }
    }

    private static IEnumerable<DataQualityIssue> CreateDataQualityIssues(
        B2Connect.Api.Validation.ValidationResult result,
        ErpData? originalData,
        string erpType,
        string tenantId,
        bool includeSql)
    {
        var issues = new List<DataQualityIssue>();

        // If the result is valid, no issues
        if (result.IsValid)
        {
            return issues;
        }

        // Create detailed issues with solutions based on the validation code
        var issue = new DataQualityIssue
        {
            Code = result.Code,
            Message = result.Message,
            FieldPath = result.FieldPath,
            Severity = result.Severity,
            RecordData = originalData,
            Solutions = GetSolutionsForCode(result.Code, erpType, originalData),
            SqlQuery = includeSql ? GetSqlQueryForIssue(result.Code, erpType, tenantId, originalData) : null
        };

        issues.Add(issue);
        return issues;
    }

    private static List<string> GetSolutionsForCode(string code, string erpType, ErpData? data)
    {
        return code switch
        {
            "MISSING_ID" => new List<string>
            {
                "Ensure each product has a unique identifier in the ERP system",
                "Check the primary key configuration in your ERP database",
                $"For {erpType}: Verify the ID field is populated during data export"
            },
            "INVALID_PRICE" => new List<string>
            {
                "Prices must be positive decimal values",
                "Check for data entry errors in the ERP system",
                "Verify currency conversion settings if applicable",
                $"For {erpType}: Ensure price field contains valid numeric data"
            },
            "INVALID_ENVENTA_SKU_FORMAT" => new List<string>
            {
                "Enventa SKU must follow format: AA123456 (2 letters + 6 digits)",
                "Update SKU format in the ERP system to match enventa requirements",
                "Contact enventa support for SKU format guidelines"
            },
            "INVALID_SAP_ID_LENGTH" => new List<string>
            {
                "SAP IDs cannot exceed 18 characters",
                "Truncate or reformat long identifiers in the ERP system",
                "Check SAP material number configuration"
            },
            _ => new List<string> { "Review data quality and correct according to ERP system requirements" }
        };
    }

    private static string? GetSqlQueryForIssue(string code, string erpType, string tenantId, ErpData? data)
    {
        if (data?.Id == null) return null;

        return erpType.ToLower() switch
        {
            "enventa" => code switch
            {
                "MISSING_ID" => $"SELECT * FROM enventa_products WHERE id IS NULL AND tenant_id = '{tenantId}';",
                "INVALID_PRICE" => $"SELECT id, price FROM enventa_products WHERE price < 0 AND tenant_id = '{tenantId}';",
                "INVALID_ENVENTA_SKU_FORMAT" => $"SELECT id, sku FROM enventa_products WHERE sku NOT REGEXP '^[A-Z]{{2}}[0-9]{{6}}$' AND tenant_id = '{tenantId}';",
                _ => $"SELECT * FROM enventa_products WHERE id = '{data.Id}' AND tenant_id = '{tenantId}';"
            },
            "sap" => code switch
            {
                "MISSING_ID" => $"SELECT * FROM sap_materials WHERE material_id IS NULL AND client = '{tenantId}';",
                "INVALID_PRICE" => $"SELECT material_id, price FROM sap_materials WHERE price < 0 AND client = '{tenantId}';",
                "INVALID_SAP_ID_LENGTH" => $"SELECT material_id FROM sap_materials WHERE LENGTH(material_id) > 18 AND client = '{tenantId}';",
                _ => $"SELECT * FROM sap_materials WHERE material_id = '{data.Id}' AND client = '{tenantId}';"
            },
            _ => null
        };
    }

    private static List<string> GenerateSqlQueries(string erpType, string tenantId)
    {
        return erpType.ToLower() switch
        {
            "enventa" => new List<string>
            {
                $"SELECT COUNT(*) as total_products FROM enventa_products WHERE tenant_id = '{tenantId}';",
                $"SELECT COUNT(*) as invalid_prices FROM enventa_products WHERE price < 0 AND tenant_id = '{tenantId}';",
                $"SELECT COUNT(*) as missing_ids FROM enventa_products WHERE id IS NULL OR id = '' AND tenant_id = '{tenantId}';",
                $"SELECT COUNT(*) as invalid_skus FROM enventa_products WHERE sku NOT REGEXP '^[A-Z]{{2}}[0-9]{{6}}$' AND tenant_id = '{tenantId}';"
            },
            "sap" => new List<string>
            {
                $"SELECT COUNT(*) as total_materials FROM sap_materials WHERE client = '{tenantId}';",
                $"SELECT COUNT(*) as invalid_prices FROM sap_materials WHERE price < 0 AND client = '{tenantId}';",
                $"SELECT COUNT(*) as missing_ids FROM sap_materials WHERE material_id IS NULL OR material_id = '' AND client = '{tenantId}';",
                $"SELECT COUNT(*) as long_ids FROM sap_materials WHERE LENGTH(material_id) > 18 AND client = '{tenantId}';"
            },
            _ => new List<string>()
        };
    }

    public class ValidationRulesProvider : IValidationRulesProvider
    {
        public Task<IEnumerable<IValidationRule<ErpData>>> GetRulesForTenantAsync(string tenantId, CancellationToken ct = default)
        {
            // Simple implementation - return empty rules for now
            return Task.FromResult(Enumerable.Empty<IValidationRule<ErpData>>());
        }

        public Task<IEnumerable<IValidationRule<ErpData>>> GetErpSpecificRulesAsync(string tenantId, string erpType, CancellationToken ct = default)
        {
            // Simple implementation - return empty rules for now
            return Task.FromResult(Enumerable.Empty<IValidationRule<ErpData>>());
        }
    }

    // Simple connector implementations for CLI validation
    public class SimpleEnventaConnector : IErpConnector
    {
        private readonly ILogger _logger;

        public string ErpType => "enventa";

        public SimpleEnventaConnector(ILogger logger)
        {
            _logger = logger;
        }

        public Task<IEnumerable<ValidationResult>> ValidateDataAsync(ErpData data, CancellationToken ct = default)
        {
            var results = new List<ValidationResult>();

            // Basic enventa-specific validation
            if (!string.IsNullOrEmpty(data.Sku) && !System.Text.RegularExpressions.Regex.IsMatch(data.Sku, @"^[A-Z]{2}\d{6}$"))
            {
                results.Add(new ValidationResult
                {
                    IsValid = false,
                    Code = "INVALID_ENVENTA_SKU",
                    Message = "SKU must match enventa format: AA123456",
                    FieldPath = "Sku",
                    Severity = ValidationSeverity.Error
                });
            }

            return Task.FromResult(results.AsEnumerable());
        }
    }

    public class SimpleSapConnector : IErpConnector
    {
        private readonly ILogger _logger;

        public string ErpType => "sap";

        public SimpleSapConnector(ILogger logger)
        {
            _logger = logger;
        }

        public Task<IEnumerable<ValidationResult>> ValidateDataAsync(ErpData data, CancellationToken ct = default)
        {
            var results = new List<ValidationResult>();

            // Basic SAP-specific validation
            if (!string.IsNullOrEmpty(data.Id) && data.Id.Length > 18)
            {
                results.Add(new ValidationResult
                {
                    IsValid = false,
                    Code = "INVALID_SAP_ID_LENGTH",
                    Message = "SAP ID cannot exceed 18 characters",
                    FieldPath = "Id",
                    Severity = ValidationSeverity.Error
                });
            }

            return Task.FromResult(results.AsEnumerable());
        }
    }

    public class DataQualityIssue
    {
        public string Code { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? FieldPath { get; set; }
        public B2Connect.Api.Validation.ValidationSeverity Severity { get; set; }
        public ErpData? RecordData { get; set; }
        public List<string> Solutions { get; set; } = new();
        public string? SqlQuery { get; set; }
    }
}