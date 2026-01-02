using B2Connect.Identity.Interfaces;
using B2Connect.Identity.Models;
using FluentValidation;

namespace B2Connect.Identity.Handlers;

/// <summary>
/// Wolverine Service Handler für CheckRegistrationTypeCommand
/// Story 8: Check Customer Type
///
/// Wolverine Pattern:
/// - Service ist ein POCO Service
/// - Wird automatisch von Wolverine in HTTP Endpoints konvertiert
/// - Methodenname wird zu HTTP Route: POST /checkregistrationtype
///
/// Ablauf:
/// 1. Duplikat-Prüfung (bestehende Accounts)
/// 2. ERP-Lookup (Bestandskunde?)
/// 3. Registrierungstyp bestimmen
/// 4. Response mit ERP-Daten (falls gefunden)
/// </summary>
public class CheckRegistrationTypeService
{
    private readonly IErpCustomerService _erpService;
    private readonly IDuplicateDetectionService _duplicateDetectionService;
    private readonly ILogger<CheckRegistrationTypeService> _logger;
    private readonly CheckRegistrationTypeCommandValidator _validator;

    public CheckRegistrationTypeService(
        IErpCustomerService erpService,
        IDuplicateDetectionService duplicateDetectionService,
        ILogger<CheckRegistrationTypeService> logger,
        CheckRegistrationTypeCommandValidator validator)
    {
        _erpService = erpService;
        _duplicateDetectionService = duplicateDetectionService;
        _logger = logger;
        _validator = validator;
    }

    /// <summary>
    /// Wolverine Service Handler für POST /api/registration/check-type
    /// </summary>
    public async Task<CheckRegistrationTypeResponse> CheckType(
        CheckRegistrationTypeCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("CheckRegistrationType gestartet für Email: {Email}, BusinessType: {BusinessType}",
            request.Email, request.BusinessType);

        try
        {
            // 1. Input Validierung
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validierungsfehler für Email: {Email}", request.Email);
                return new CheckRegistrationTypeResponse
                {
                    Success = false,
                    RegistrationType = RegistrationType.NewCustomer,
                    Error = "VALIDATION_ERROR",
                    Message = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage))
                };
            }

            // 2. Duplikat-Prüfung zuerst
            var duplicateCheck = await _duplicateDetectionService.CheckForDuplicatesAsync(
                request.Email,
                request.FirstName,
                request.LastName,
                request.CompanyName,
                request.Phone,
                cancellationToken);

            if (duplicateCheck.ShouldBlock && duplicateCheck.ConfidenceScore >= 90)
            {
                _logger.LogWarning("Duplikat erkannt für Email: {Email} (Score: {Score})",
                    request.Email, duplicateCheck.ConfidenceScore);

                return new CheckRegistrationTypeResponse
                {
                    Success = false,
                    RegistrationType = RegistrationType.NewCustomer,
                    Error = "ACCOUNT_EXISTS",
                    Message = "Ein Konto mit dieser E-Mail-Adresse existiert bereits."
                };
            }

            // 3. ERP-Lookup durchführen
            ErpCustomerDto? erpCustomer = null;

            // Zuerst nach Kundennummer suchen (wenn vorhanden)
            if (!string.IsNullOrWhiteSpace(request.CustomerNumber))
            {
                erpCustomer = await _erpService.GetCustomerByNumberAsync(
                    request.CustomerNumber, cancellationToken);

                if (erpCustomer != null)
                {
                    _logger.LogInformation("ERP-Kunde gefunden nach Kundennummer: {CustomerNumber}",
                        request.CustomerNumber);
                }
            }

            // Dann nach E-Mail suchen (wenn noch nicht gefunden)
            if (erpCustomer == null)
            {
                erpCustomer = await _erpService.GetCustomerByEmailAsync(
                    request.Email, cancellationToken);

                if (erpCustomer != null)
                {
                    _logger.LogInformation("ERP-Kunde gefunden nach E-Mail: {Email}", request.Email);
                }
            }

            // Für B2B: nach Firmenname suchen
            if (erpCustomer == null && request.BusinessType == "BUSINESS" &&
                !string.IsNullOrWhiteSpace(request.CompanyName))
            {
                erpCustomer = await _erpService.GetCustomerByCompanyNameAsync(
                    request.CompanyName, cancellationToken);

                if (erpCustomer != null)
                {
                    _logger.LogInformation("ERP-Kunde gefunden nach Firmenname: {CompanyName}",
                        request.CompanyName);
                }
            }

            // 4. Registrierungstyp bestimmen
            var registrationType = erpCustomer != null ? RegistrationType.ExistingCustomer : RegistrationType.NewCustomer;

            if (erpCustomer != null && request.BusinessType == "BUSINESS")
            {
                registrationType = RegistrationType.BusinessCustomer;
            }

            _logger.LogInformation("Registrierungstyp bestimmt: {RegistrationType}", registrationType);

            // 5. Response mit ERP-Daten zusammenstellen
            var response = new CheckRegistrationTypeResponse
            {
                Success = true,
                RegistrationType = registrationType,
                Message = registrationType switch
                {
                    RegistrationType.ExistingCustomer =>
                        $"Willkommen zurück! Wir haben Ihren Kundenaccount im System gefunden (Kundennummer: {erpCustomer!.CustomerNumber}). Bitte verwenden Sie die vereinfachte Registrierung.",
                    RegistrationType.BusinessCustomer =>
                        $"Geschäftskunde erkannt: {erpCustomer!.CustomerName}. Bitte kompletieren Sie Ihre Geschäftsdaten.",
                    _ => "Sie werden als Neukunde registriert."
                }
            };

            // ERP-Daten hinzufügen, wenn Bestandskunde
            if (erpCustomer != null)
            {
                response.Data = new RegistrationTypeResponseDto
                {
                    RegistrationType = registrationType,
                    IsExistingCustomer = true,
                    ErpCustomerId = erpCustomer.CustomerNumber,
                    ErpCustomerName = erpCustomer.CustomerName,
                    ErpCustomerAddress = erpCustomer.ShippingAddress,
                    ErpCustomerCountry = erpCustomer.Country,
                    ErpBillingAddress = erpCustomer.BillingAddress,
                    ErpShippingAddress = erpCustomer.ShippingAddress,
                    BusinessType = erpCustomer.BusinessType,
                    MatchConfidenceScore = 95  // ERP-Match ist höchste Konfidenz
                };
            }

            _logger.LogInformation("CheckRegistrationType erfolgreich: {Email} -> {RegistrationType}",
                request.Email, response.RegistrationType);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fehler bei CheckRegistrationType für Email: {Email}",
                request.Email);

            return new CheckRegistrationTypeResponse
            {
                Success = false,
                RegistrationType = RegistrationType.NewCustomer,
                Error = "INTERNAL_ERROR",
                Message = "Ein Fehler ist bei der Überprüfung aufgetreten. Bitte versuchen Sie es später erneut."
            };
        }
    }
}
