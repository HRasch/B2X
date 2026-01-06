using B2Connect.LocalizationService.Data;
using B2Connect.LocalizationService.Models;
using Microsoft.EntityFrameworkCore;

namespace B2Connect.LocalizationService.Data;

/// <summary>
/// Seeder for initializing localization database with base translations
/// </summary>
public static class LocalizationSeeder
{
    /// <summary>
    /// Seeds the database with initial localized strings
    /// </summary>
    public static async Task SeedAsync(LocalizationDbContext dbContext)
    {
        if (await dbContext.LocalizedStrings.AnyAsync())
        {
            return; // Already seeded
        }

        var strings = new[]
        {
            // Auth category
            new LocalizedString
            {
                Id = Guid.NewGuid(),
                Key = "login",
                Category = "auth",
                DefaultValue = "Login",
                Translations = new Dictionary<string, string>
                {
                    { "en", "Login" },
                    { "de", "Anmelden" },
                    { "fr", "Connexion" },
                    { "es", "Iniciar Sesión" },
                    { "it", "Accedi" },
                    { "pt", "Entrar" },
                    { "nl", "Aanmelden" },
                    { "pl", "Zaloguj się" }
                },
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new LocalizedString
            {
                Id = Guid.NewGuid(),
                Key = "logout",
                Category = "auth",
                DefaultValue = "Logout",
                Translations = new Dictionary<string, string>
                {
                    { "en", "Logout" },
                    { "de", "Abmelden" },
                    { "fr", "Déconnexion" },
                    { "es", "Cerrar Sesión" },
                    { "it", "Esci" },
                    { "pt", "Sair" },
                    { "nl", "Afmelden" },
                    { "pl", "Wyloguj się" }
                },
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new LocalizedString
            {
                Id = Guid.NewGuid(),
                Key = "register",
                Category = "auth",
                DefaultValue = "Register",
                Translations = new Dictionary<string, string>
                {
                    { "en", "Register" },
                    { "de", "Registrieren" },
                    { "fr", "S'enregistrer" },
                    { "es", "Registrarse" },
                    { "it", "Registrati" },
                    { "pt", "Registrar" },
                    { "nl", "Registreren" },
                    { "pl", "Zarejestruj się" }
                },
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },

            // UI category
            new LocalizedString
            {
                Id = Guid.NewGuid(),
                Key = "save",
                Category = "ui",
                DefaultValue = "Save",
                Translations = new Dictionary<string, string>
                {
                    { "en", "Save" },
                    { "de", "Speichern" },
                    { "fr", "Enregistrer" },
                    { "es", "Guardar" },
                    { "it", "Salva" },
                    { "pt", "Salvar" },
                    { "nl", "Opslaan" },
                    { "pl", "Zapisz" }
                },
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new LocalizedString
            {
                Id = Guid.NewGuid(),
                Key = "cancel",
                Category = "ui",
                DefaultValue = "Cancel",
                Translations = new Dictionary<string, string>
                {
                    { "en", "Cancel" },
                    { "de", "Abbrechen" },
                    { "fr", "Annuler" },
                    { "es", "Cancelar" },
                    { "it", "Annulla" },
                    { "pt", "Cancelar" },
                    { "nl", "Annuleren" },
                    { "pl", "Anuluj" }
                },
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new LocalizedString
            {
                Id = Guid.NewGuid(),
                Key = "delete",
                Category = "ui",
                DefaultValue = "Delete",
                Translations = new Dictionary<string, string>
                {
                    { "en", "Delete" },
                    { "de", "Löschen" },
                    { "fr", "Supprimer" },
                    { "es", "Eliminar" },
                    { "it", "Elimina" },
                    { "pt", "Deletar" },
                    { "nl", "Verwijderen" },
                    { "pl", "Usuń" }
                },
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new LocalizedString
            {
                Id = Guid.NewGuid(),
                Key = "next",
                Category = "ui",
                DefaultValue = "Next",
                Translations = new Dictionary<string, string>
                {
                    { "en", "Next" },
                    { "de", "Weiter" },
                    { "fr", "Suivant" },
                    { "es", "Siguiente" },
                    { "it", "Avanti" },
                    { "pt", "Próximo" },
                    { "nl", "Volgende" },
                    { "pl", "Dalej" }
                },
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new LocalizedString
            {
                Id = Guid.NewGuid(),
                Key = "previous",
                Category = "ui",
                DefaultValue = "Previous",
                Translations = new Dictionary<string, string>
                {
                    { "en", "Previous" },
                    { "de", "Zurück" },
                    { "fr", "Précédent" },
                    { "es", "Anterior" },
                    { "it", "Indietro" },
                    { "pt", "Anterior" },
                    { "nl", "Vorige" },
                    { "pl", "Wstecz" }
                },
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },

            // Errors category
            new LocalizedString
            {
                Id = Guid.NewGuid(),
                Key = "required",
                Category = "errors",
                DefaultValue = "This field is required",
                Translations = new Dictionary<string, string>
                {
                    { "en", "This field is required" },
                    { "de", "Dieses Feld ist erforderlich" },
                    { "fr", "Ce champ est obligatoire" },
                    { "es", "Este campo es obligatorio" },
                    { "it", "Questo campo è obbligatorio" },
                    { "pt", "Este campo é obrigatório" },
                    { "nl", "Dit veld is verplicht" },
                    { "pl", "To pole jest wymagane" }
                },
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new LocalizedString
            {
                Id = Guid.NewGuid(),
                Key = "invalid_email",
                Category = "errors",
                DefaultValue = "Invalid email address",
                Translations = new Dictionary<string, string>
                {
                    { "en", "Invalid email address" },
                    { "de", "Ungültige E-Mail-Adresse" },
                    { "fr", "Adresse e-mail invalide" },
                    { "es", "Dirección de correo electrónico no válida" },
                    { "it", "Indirizzo e-mail non valido" },
                    { "pt", "Endereço de e-mail inválido" },
                    { "nl", "Ongeldig e-mailadres" },
                    { "pl", "Nieprawidłowy adres e-mail" }
                },
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new LocalizedString
            {
                Id = Guid.NewGuid(),
                Key = "unauthorized",
                Category = "errors",
                DefaultValue = "You are not authorized to perform this action",
                Translations = new Dictionary<string, string>
                {
                    { "en", "You are not authorized to perform this action" },
                    { "de", "Sie sind nicht berechtigt, diese Aktion auszuführen" },
                    { "fr", "Vous n'êtes pas autorisé à effectuer cette action" },
                    { "es", "No está autorizado para realizar esta acción" },
                    { "it", "Non sei autorizzato a eseguire questa azione" },
                    { "pt", "Você não está autorizado a executar esta ação" },
                    { "nl", "U bent niet geautoriseerd om deze actie uit te voeren" },
                    { "pl", "Nie jesteś uprawniony do wykonania tej akcji" }
                },
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
        };

        await dbContext.LocalizedStrings.AddRangeAsync(strings);
        await dbContext.SaveChangesAsync();
    }
}
