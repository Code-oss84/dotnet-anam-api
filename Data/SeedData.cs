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

        if (!context.Formations.Any())
        {
            var devWeb = context.Categories.First(c => c.Nom == "Développement Web");
            var dataSci = context.Categories.First(c => c.Nom == "Data Science");
            var cyber = context.Categories.First(c => c.Nom == "Cybersécurité");
            var marie = context.Formateurs.First(f => f.Email == "marie.dupont@formations.com");
            var pierre = context.Formateurs.First(f => f.Email == "pierre.martin@formations.com");
            var sophie = context.Formateurs.First(f => f.Email == "sophie.bernard@formations.com");

            var f1 = new Formation
            {
                Titre = "Développement Web Full Stack",
                Description = "Maîtrisez le développement web front-end et back-end avec React, ASP.NET Core et les bases de données modernes.",
                DureeHeures = 120, Prix = 4500, DateDebut = new DateTime(2026, 9, 1),
                DateFin = new DateTime(2026, 12, 15), NombrePlaces = 30, EstActive = true,
                CategorieId = devWeb.Id, FormateurId = marie.Id
            };
            var f2 = new Formation
            {
                Titre = "Data Science & Machine Learning",
                Description = "Apprenez à analyser des données et à construire des modèles de machine learning avec Python, Pandas et Scikit-learn.",
                DureeHeures = 100, Prix = 5000, DateDebut = new DateTime(2026, 9, 1),
                DateFin = new DateTime(2026, 12, 10), NombrePlaces = 25, EstActive = true,
                CategorieId = dataSci.Id, FormateurId = pierre.Id
            };
            var f3 = new Formation
            {
                Titre = "Cybersécurité Pratique",
                Description = "Découvrez les techniques de sécurité informatique, le pentesting et la protection des systèmes d'information.",
                DureeHeures = 80, Prix = 5500, DateDebut = new DateTime(2026, 10, 1),
                DateFin = new DateTime(2027, 1, 20), NombrePlaces = 20, EstActive = true,
                CategorieId = cyber.Id, FormateurId = sophie.Id
            };
            context.Formations.AddRange(f1, f2, f3);
            await context.SaveChangesAsync();

            context.Modules.AddRange(
                new Module { Titre = "Introduction à HTML & CSS", Description = "Bases du HTML5 et CSS3 pour créer des pages web.", Ordre = 1, DureeHeures = 15, FormationId = f1.Id },
                new Module { Titre = "JavaScript & React", Description = "Programmation client avec JavaScript moderne et framework React.", Ordre = 2, DureeHeures = 30, FormationId = f1.Id },
                new Module { Titre = "ASP.NET Core & API REST", Description = "Création d'APIs back-end robustes avec ASP.NET Core.", Ordre = 3, DureeHeures = 35, FormationId = f1.Id },
                new Module { Titre = "Bases de données SQL & Entity Framework", Description = "Modélisation de données et ORM avec Entity Framework Core.", Ordre = 4, DureeHeures = 25, FormationId = f1.Id },
                new Module { Titre = "Introduction à Python pour la Data", Description = "Python, NumPy, Pandas et Visualisation de données.", Ordre = 1, DureeHeures = 20, FormationId = f2.Id },
                new Module { Titre = "Statistiques & Probabilités", Description = "Fondamentaux statistiques pour l'analyse de données.", Ordre = 2, DureeHeures = 20, FormationId = f2.Id },
                new Module { Titre = "Machine Learning Supervisé", Description = "Régression, classification, arbres de décision avec Scikit-learn.", Ordre = 3, DureeHeures = 30, FormationId = f2.Id },
                new Module { Titre = "Projets Pratiques Data Science", Description = "Projet complet de bout en bout sur des données réelles.", Ordre = 4, DureeHeures = 30, FormationId = f2.Id },
                new Module { Titre = "Fondamentaux de la Sécurité", Description = "Principes de la sécurité informatique, menaces et vecteurs d'attaque.", Ordre = 1, DureeHeures = 20, FormationId = f3.Id },
                new Module { Titre = "Pentesting & Ethical Hacking", Description = "Techniques de tests de pénétration et exploitation de vulnérabilités.", Ordre = 2, DureeHeures = 30, FormationId = f3.Id },
                new Module { Titre = "Sécurité Réseau & Pare-feu", Description = "Configuration de pare-feu, VPN et sécurisation des communications.", Ordre = 3, DureeHeures = 15, FormationId = f3.Id },
                new Module { Titre = "Veille & Réponse aux Incidents", Description = "Monitoring, détection d'intrusion et gestion de incidents de sécurité.", Ordre = 4, DureeHeures = 15, FormationId = f3.Id }
            );
            await context.SaveChangesAsync();
        }
    }
}
