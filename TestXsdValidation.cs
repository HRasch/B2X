using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using B2Connect.Catalog.Application.Adapters;
using B2Connect.Catalog.Application.Services;
using B2Connect.Catalog.Domain.Models;
using Microsoft.Extensions.Logging;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Testing BMEcat XSD Validation...");

        // Create a simple logger
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var logger = loggerFactory.CreateLogger<BmecatImportAdapter>();

        // Create the adapter
        var adapter = new BmecatImportAdapter(logger);

        // Test with the sample file
        using var fileStream = File.OpenRead("test-data/test-bmecat-1.2.xml");

        var metadata = new CatalogMetadata
        {
            TenantId = "test-tenant",
            FileName = "test-bmecat-1.2.xml",
            ContentType = "application/xml"
        };

        var result = new CatalogImportResult();

        try
        {
            // This should now use XSD validation
            var (catalogId, products) = await adapter.ImportAsync(fileStream, metadata, result, CancellationToken.None);

            Console.WriteLine($"Import Result: Success={result.Success}");
            Console.WriteLine($"Catalog ID: {catalogId}");
            Console.WriteLine($"Products Count: {products?.Count ?? 0}");
            Console.WriteLine($"Validation Errors: {result.ValidationErrors.Count}");

            if (result.ValidationErrors.Count > 0)
            {
                Console.WriteLine("Validation Errors:");
                foreach (var error in result.ValidationErrors)
                {
                    Console.WriteLine($"  - {error}");
                }
            }

            if (!string.IsNullOrEmpty(result.ErrorMessage))
            {
                Console.WriteLine($"Error Message: {result.ErrorMessage}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
            Console.WriteLine($"Stack Trace: {ex.StackTrace}");
        }
    }
}