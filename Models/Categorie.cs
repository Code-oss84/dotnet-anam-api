using System.ComponentModel.DataAnnotations;

namespace GestionFormations.Models;

public class Categorie
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Le nom est requis")]
    [StringLength(100)]
    [Display(Name = "Nom de la catégorie")]
    public string Nom { get; set; } = string.Empty;

    [StringLength(500)]
    [Display(Name = "Description")]
    public string? Description { get; set; }

    public ICollection<Formation> Formations { get; set; } = new List<Formation>();
}
