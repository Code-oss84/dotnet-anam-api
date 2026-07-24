using System.ComponentModel.DataAnnotations;

namespace GestionFormations.Models;

public class Formation
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Le titre est requis")]
    [StringLength(200)]
    public string Titre { get; set; } = string.Empty;

    [StringLength(2000)]
    public string? Description { get; set; }

    [Display(Name = "Durée (heures)")]
    [Range(1, 1000)]
    public int DureeHeures { get; set; }

    [Display(Name = "Prix")]
    [Range(0, 999999)]
    public decimal Prix { get; set; }

    [Display(Name = "Date de début")]
    [DataType(DataType.Date)]
    public DateTime DateDebut { get; set; }

    [Display(Name = "Date de fin")]
    [DataType(DataType.Date)]
    public DateTime DateFin { get; set; }

    [Display(Name = "Nombre de places")]
    [Range(1, 10000)]
    public int NombrePlaces { get; set; }

    [Display(Name = "Image URL")]
    public string? ImageUrl { get; set; }

    public bool EstActive { get; set; } = true;

    [Display(Name = "Catégorie")]
    public int CategorieId { get; set; }
    public Categorie? Categorie { get; set; }

    [Display(Name = "Formateur")]
    public int? FormateurId { get; set; }
    public Formateur? Formateur { get; set; }

    public ICollection<Module> Modules { get; set; } = new List<Module>();
    public ICollection<Inscription> Inscriptions { get; set; } = new List<Inscription>();
}
