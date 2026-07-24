using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace GestionFormations.Models;

public class Apprenant : IdentityUser
{
    [Required(ErrorMessage = "Le nom est requis")]
    [StringLength(100)]
    public string Nom { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le prénom est requis")]
    [StringLength(100)]
    public string Prenom { get; set; } = string.Empty;

    [Display(Name = "Photo URL")]
    public string? PhotoUrl { get; set; }

    [Display(Name = "Date de naissance")]
    [DataType(DataType.Date)]
    public DateTime? DateNaissance { get; set; }

    public bool EstActif { get; set; } = true;

    public ICollection<Inscription> Inscriptions { get; set; } = new List<Inscription>();
    public ICollection<ReponseApprenant> Reponses { get; set; } = new List<ReponseApprenant>();
}
