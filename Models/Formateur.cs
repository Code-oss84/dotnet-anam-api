using System.ComponentModel.DataAnnotations;

namespace GestionFormations.Models;

public class Formateur
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Le nom est requis")]
    [StringLength(100)]
    public string Nom { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le prénom est requis")]
    [StringLength(100)]
    public string Prenom { get; set; } = string.Empty;

    [Required(ErrorMessage = "L'email est requis")]
    [EmailAddress(ErrorMessage = "Email invalide")]
    public string Email { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Téléphone invalide")]
    [Display(Name = "Téléphone")]
    public string? Telephone { get; set; }

    [StringLength(500)]
    public string? Specialite { get; set; }

    [Display(Name = "Photo URL")]
    public string? PhotoUrl { get; set; }

    [Display(Name = "Biographie")]
    public string? Biographie { get; set; }

    public bool EstActif { get; set; } = true;

    public ICollection<Formation> Formations { get; set; } = new List<Formation>();
}
