using System;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using Microsoft.AspNetCore.Identity;
using B2Connect.AuthService.Data;
using B2Connect.Admin.Infrastructure.Data;
using B2Connect.Shared.Core;

namespace B2Connect.Tests.Utilities;

/// <summary>
/// Utility methods to seed in-memory test databases with bogus/demo data.
/// Call these from test fixtures to populate Auth and Catalog in-memory contexts.
/// </summary>
public static class InMemoryTestSeeder
{
    /// <summary>
    /// Seeds the Identity store with bogus users and ensures an admin user exists.
    /// </summary>
    public static async Task SeedAuthAsync(UserManager<AppUser> userManager, int userCount = 5)
    {
        if (userManager == null) throw new ArgumentNullException(nameof(userManager));

        var faker = new Faker();

        // Create a few random users
        for (int i = 0; i < userCount; i++)
        {
            var email = faker.Internet.Email();
            if (await userManager.FindByEmailAsync(email) != null) continue;

            var user = new AppUser
            {
                UserName = email,
                Email = email,
                FirstName = faker.Name.FirstName(),
                LastName = faker.Name.LastName(),
                TenantId = SeedConstants.DefaultTenantId.ToString(),
                IsActive = true,
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            await userManager.CreateAsync(user, "Password123!");
        }

        // Ensure admin user exists (tests rely on this sometimes)
        var adminEmail = SeedConstants.DefaultAdminEmail;
        var admin = await userManager.FindByEmailAsync(adminEmail);
        if (admin == null)
        {
            admin = new AppUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FirstName = "Admin",
                LastName = "User",
                TenantId = SeedConstants.DefaultTenantId.ToString(),
                IsActive = true,
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            await userManager.CreateAsync(admin, "AdminPassword123!");
        }

        // Add admin role if available
        try
        {
            var roles = await userManager.GetRolesAsync(admin);
            if (!roles.Contains("Admin"))
            {
                // This will no-op if RoleManager isn't configured; tests that require roles should ensure RoleManager
                await userManager.AddToRoleAsync(admin, "Admin");
            }
        }
        catch
        {
            // Ignore role assignment failures in minimal test setups
        }
    }

    /// <summary>
    /// Seeds a CatalogDbContext (in-memory) using the existing CatalogDemoDataGenerator.
    /// </summary>
    public static void SeedCatalogDemo(CatalogDbContext context, int productCount = 50)
    {
        if (context == null) throw new ArgumentNullException(nameof(context));

        if (context.Products.Any()) return; // already seeded

        var (categories, brands, products) = CatalogDemoDataGenerator.GenerateDemoCatalog(productCount);

        context.Categories.AddRange(categories);
        context.Brands.AddRange(brands);
        context.Products.AddRange(products);
        context.SaveChanges();
    }
}
