using Microsoft.EntityFrameworkCore;
using B2Connect.LocalizationService.Data;
using B2Connect.LocalizationService.Models;

namespace B2Connect.LocalizationService.Data;

/// <summary>
/// Seeder for initializing localization database with base translations
/// </summary>
public static class LocalizationSeeder
{
    private static LocalizedStringEntity CreateLocalizedStringEntity(
        string key,
        string category,
        string defaultValue,
        Dictionary<string, string> translations,
        Guid tenantId = default)
    {
        return new LocalizedStringEntity(
            tenantId: tenantId,
            localizedString: new LocalizedString(key, category, defaultValue, translations)
        );
    }

    /// <summary>
    /// Seeds the database with initial localized strings
    /// </summary>
    public static async Task SeedAsync(LocalizationDbContext dbContext)
    {
        if (await dbContext.LocalizedStringEntities.CountAsync() > 0)
        {
            return; // Already seeded
        }

        var strings = new[]
        {
            // Auth category
            CreateLocalizedStringEntity(
                key: "login",
                category: "auth",
                defaultValue: "Login",
                translations: new Dictionary<string, string>
                {
                    { "en", "Login" },
                    { "de", "Anmelden" },
                    { "fr", "Connexion" },
                    { "es", "Iniciar Sesión" },
                    { "it", "Accedi" },
                    { "pt", "Entrar" },
                    { "nl", "Aanmelden" },
                    { "pl", "Zaloguj się" }
                }
            ),
            CreateLocalizedStringEntity(
                key: "logout",
                category: "auth",
                defaultValue: "Logout",
                translations: new Dictionary<string, string>
                {
                    { "en", "Logout" },
                    { "de", "Abmelden" },
                    { "fr", "Déconnexion" },
                    { "es", "Cerrar Sesión" },
                    { "it", "Esci" },
                    { "pt", "Sair" },
                    { "nl", "Afmelden" },
                    { "pl", "Wyloguj się" }
                }
            ),
            CreateLocalizedStringEntity(
                key: "register",
                category: "auth",
                defaultValue: "Register",
                translations: new Dictionary<string, string>
                {
                    { "en", "Register" },
                    { "de", "Registrieren" },
                    { "fr", "S'inscrire" },
                    { "es", "Registrarse" },
                    { "it", "Registrati" },
                    { "pt", "Registrar" },
                    { "nl", "Registreren" },
                    { "pl", "Zarejestruj się" }
                }
            ),
            CreateLocalizedStringEntity(
                key: "save",
                category: "ui",
                defaultValue: "Save",
                translations: new Dictionary<string, string>
                {
                    { "en", "Save" },
                    { "de", "Speichern" },
                    { "fr", "Enregistrer" },
                    { "es", "Guardar" },
                    { "it", "Salva" },
                    { "pt", "Salvar" },
                    { "nl", "Opslaan" },
                    { "pl", "Zapisz" }
                }
            ),
            CreateLocalizedStringEntity(
                key: "cancel",
                category: "ui",
                defaultValue: "Cancel",
                translations: new Dictionary<string, string>
                {
                    { "en", "Cancel" },
                    { "de", "Abbrechen" },
                    { "fr", "Annuler" },
                    { "es", "Cancelar" },
                    { "it", "Annulla" },
                    { "pt", "Cancelar" },
                    { "nl", "Annuleren" },
                    { "pl", "Anuluj" }
                }
            )
        };

        await dbContext.LocalizedStringEntities.AddRangeAsync(strings);
        await dbContext.SaveChangesAsync();
    }
}
