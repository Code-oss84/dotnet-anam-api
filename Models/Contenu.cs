using System.ComponentModel.DataAnnotations;

namespace GestionFormations.Models;

public enum TypeContenu
{
    Cours,
    Exercice,
    Examen,
    Quiz
}

public class Contenu
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Le titre est requis")]
    [StringLength(200)]
    public string Titre { get; set; } = string.Empty;

    [StringLength(5000)]
    public string? Description { get; set; }

    [Display(Name = "Type de contenu")]
    public TypeContenu Type { get; set; }

    [Display(Name = "Contenu textuel")]
    public string? Texte { get; set; }

    [Display(Name = "URL de la vidéo")]
    public string? VideoUrl { get; set; }

    [Display(Name = "URL du document")]
    public string? DocumentUrl { get; set; }

    [Display(Name = "Durée (minutes)")]
    [Range(1, 600)]
    public int DureeMinutes { get; set; }

    [Display(Name = "Ordre")]
    [Range(1, 100)]
    public int Ordre { get; set; }

    [Display(Name = "Module")]
    public int ModuleId { get; set; }
    public Module? Module { get; set; }

    public ICollection<Question> Questions { get; set; } = new List<Question>();
}
