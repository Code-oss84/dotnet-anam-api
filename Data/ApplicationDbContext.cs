using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using GestionFormations.Models;

namespace GestionFormations.Data;

public class ApplicationDbContext : IdentityDbContext<Apprenant>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Categorie> Categories => Set<Categorie>();
    public DbSet<Formateur> Formateurs => Set<Formateur>();
    public DbSet<Formation> Formations => Set<Formation>();
    public DbSet<Module> Modules => Set<Module>();
    public DbSet<Contenu> Contenus => Set<Contenu>();
    public DbSet<Question> Questions => Set<Question>();
    public DbSet<ChoixReponse> ChoixReponses => Set<ChoixReponse>();
    public DbSet<Inscription> Inscriptions => Set<Inscription>();
    public DbSet<ReponseApprenant> ReponsesApprenants => Set<ReponseApprenant>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Formation>()
            .HasOne(f => f.Categorie)
            .WithMany(c => c.Formations)
            .HasForeignKey(f => f.CategorieId);

        builder.Entity<Formation>()
            .HasOne(f => f.Formateur)
            .WithMany(fo => fo.Formations)
            .HasForeignKey(f => f.FormateurId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<Module>()
            .HasOne(m => m.Formation)
            .WithMany(f => f.Modules)
            .HasForeignKey(m => m.FormationId);

        builder.Entity<Contenu>()
            .HasOne(c => c.Module)
            .WithMany(m => m.Contenus)
            .HasForeignKey(c => c.ModuleId);

        builder.Entity<Question>()
            .HasOne(q => q.Contenu)
            .WithMany(c => c.Questions)
            .HasForeignKey(q => q.ContenuId);

        builder.Entity<ChoixReponse>()
            .HasOne(cr => cr.Question)
            .WithMany(q => q.ChoixReponses)
            .HasForeignKey(cr => cr.QuestionId);

        builder.Entity<Inscription>()
            .HasOne(i => i.Formation)
            .WithMany(f => f.Inscriptions)
            .HasForeignKey(i => i.FormationId);

        builder.Entity<Inscription>()
            .HasOne(i => i.Apprenant)
            .WithMany(a => a.Inscriptions)
            .HasForeignKey(i => i.ApprenantId);

        builder.Entity<ReponseApprenant>()
            .HasOne(r => r.Question)
            .WithMany()
            .HasForeignKey(r => r.QuestionId);

        builder.Entity<ReponseApprenant>()
            .HasOne(r => r.ChoixReponse)
            .WithMany()
            .HasForeignKey(r => r.ChoixReponseId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<ReponseApprenant>()
            .HasOne(r => r.Apprenant)
            .WithMany(a => a.Reponses)
            .HasForeignKey(r => r.ApprenantId);
    }
}
