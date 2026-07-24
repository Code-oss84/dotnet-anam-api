using System.ComponentModel.DataAnnotations;

namespace GestionFormations.Models;

public class Module
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Le titre est requis")]
    [StringLength(200)]
    public string Titre { get; set; } = string.Empty;

    [StringLength(2000)]
    public string? Description { get; set; }

    [Display(Name = "Ordre")]
    [Range(1, 100)]
    public int Ordre { get; set; }

    [Display(Name = "Durée (heures)")]
    [Range(1, 500)]
    public int DureeHeures { get; set; }

    [Display(Name = "Formation")]
    public int FormationId { get; set; }
    public Formation? Formation { get; set; }

    public ICollection<Contenu> Contenus { get; set; } = new List<Contenu>();
}
