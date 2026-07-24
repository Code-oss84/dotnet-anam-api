using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using GestionFormations.Models;

namespace GestionFormations.Data;

public static class SeedData
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        using var context = new ApplicationDbContext(
            serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());

        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<Apprenant>>();

        string[] roles = { "Admin", "Formateur", "Apprenant" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        var adminEmail = "admin@gestionformations.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new Apprenant
            {
                UserName = adminEmail,
                Email = adminEmail,
                Nom = "Admin",
                Prenom = "Super",
                EmailConfirmed = true,
                EstActif = true
            };
            var result = await userManager.CreateAsync(adminUser, "Admin@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }

        if (!context.Categories.Any())
        {
            context.Categories.AddRange(
                new Categorie { Nom = "Développement Web", Description = "Formations en développement web et frameworks" },
                new Categorie { Nom = "Data Science", Description = "Formations en science des données et IA" },
                new Categorie { Nom = "Cybersécurité", Description = "Formations en sécurité informatique" },
                new Categorie { Nom = "Cloud & DevOps", Description = "Formations en cloud computing et DevOps" },
                new Categorie { Nom = "Design UX/UI", Description = "Formations en design et expérience utilisateur" }
            );
            await context.SaveChangesAsync();
        }

        if (!context.Formateurs.Any())
        {
            context.Formateurs.AddRange(
                new Formateur { Nom = "Dupont", Prenom = "Marie", Email = "marie.dupont@formations.com", Specialite = "Développement Web / React / Angular", EstActif = true },
                new Formateur { Nom = "Martin", Prenom = "Pierre", Email = "pierre.martin@formations.com", Specialite = "Data Science / Python / Machine Learning", EstActif = true },
                new Formateur { Nom = "Bernard", Prenom = "Sophie", Email = "sophie.bernard@formations.com", Specialite = "Cybersécurité / Réseau", EstActif = true }
            );
            await context.SaveChangesAsync();
        }
    }
}
