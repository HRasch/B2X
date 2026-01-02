using B2Connect.Identity.Models;
using FluentValidation;

namespace B2Connect.Identity.Handlers;

/// <summary>
/// Wolverine Command: Prüfung des Registrierungstyps (Neu- oder Bestandskunde)
/// Story 8: Check Customer Type
///
/// Wolverine Pattern:
/// - Command ist ein POCO (Plain Old C# Object)
/// - Handler hat [WolverineHandler] Attribute
/// - Handler Methode ist Async und gibt das Response-Objekt zurück
/// </summary>
public class CheckRegistrationTypeCommand
{
    /// <summary>
    /// Kundennummer im ERP
    /// </summary>
    public string? CustomerNumber { get; set; }

    /// <summary>
    /// E-Mail-Adresse
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Vorname (für Duplikat-Prüfung)
    /// </summary>
    public string? FirstName { get; set; }

    /// <summary>
    /// Nachname (für Duplikat-Prüfung)
    /// </summary>
    public string? LastName { get; set; }

    /// <summary>
    /// Firmenname (für B2B)
    /// </summary>
    public string? CompanyName { get; set; }

    /// <summary>
    /// Telefonnummer (für Duplikat-Prüfung)
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// Geschäftstyp (PRIVATE oder BUSINESS)
    /// </summary>
    public string BusinessType { get; set; } = "PRIVATE";

    /// <summary>
    /// Quelle der Registrierung
    /// </summary>
    public RegistrationSource Source { get; set; } = RegistrationSource.PublicWebsite;
}

/// <summary>
/// Response für Check Registration Type
/// </summary>
public class CheckRegistrationTypeResponse
{
    /// <summary>
    /// War die Prüfung erfolgreich?
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Erkannter Registrierungstyp
    /// </summary>
    public RegistrationType RegistrationType { get; set; }

    /// <summary>
    /// ERP-Kundendaten (wenn Bestandskunde)
    /// </summary>
    public RegistrationTypeResponseDto? Data { get; set; }

    /// <summary>
    /// Fehlermeldung (falls vorhanden)
    /// </summary>
    public string? Error { get; set; }

    /// <summary>
    /// Benutzer-freundliche Nachricht
    /// </summary>
    public string? Message { get; set; }
}

/// <summary>
/// Validator für CheckRegistrationTypeCommand (FluentValidation)
/// </summary>
public class CheckRegistrationTypeCommandValidator : AbstractValidator<CheckRegistrationTypeCommand>
{
    public CheckRegistrationTypeCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("E-Mail ist erforderlich")
            .EmailAddress().WithMessage("Gültige E-Mail-Adresse erforderlich")
            .MaximumLength(256).WithMessage("E-Mail zu lang");

        RuleFor(x => x.FirstName)
            .MaximumLength(100).WithMessage("Vorname zu lang");

        RuleFor(x => x.LastName)
            .MaximumLength(100).WithMessage("Nachname zu lang");

        RuleFor(x => x.CompanyName)
            .MaximumLength(255).WithMessage("Firmenname zu lang");

        RuleFor(x => x.Phone)
            .MaximumLength(20).WithMessage("Telefonnummer zu lang");

        RuleFor(x => x.BusinessType)
            .Must(x => x == "PRIVATE" || x == "BUSINESS")
            .WithMessage("Geschäftstyp muss PRIVATE oder BUSINESS sein");

        // Kundennummer optional, aber wenn vorhanden dann validieren
        When(x => !string.IsNullOrWhiteSpace(x.CustomerNumber), () =>
        {
            RuleFor(x => x.CustomerNumber)
                .MaximumLength(50).WithMessage("Kundennummer zu lang")
                .Matches(@"^[A-Z0-9\-]*$").WithMessage("Kundennummer darf nur Zahlen, Buchstaben und Bindestriche enthalten");
        });
    }
}
