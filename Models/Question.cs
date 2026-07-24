using System.ComponentModel.DataAnnotations;

namespace GestionFormations.Models;

public class Question
{
    public int Id { get; set; }

    [Required(ErrorMessage = "L'énoncé est requis")]
    [StringLength(1000)]
    public string Enonce { get; set; } = string.Empty;

    [Display(Name = "Type de question")]
    public TypeQuestion Type { get; set; }

    [Display(Name = "Points")]
    [Range(1, 100)]
    public int Points { get; set; }

    [Display(Name = "Contenu")]
    public int ContenuId { get; set; }
    public Contenu? Contenu { get; set; }

    public ICollection<ChoixReponse> ChoixReponses { get; set; } = new List<ChoixReponse>();
}

public enum TypeQuestion
{
    QCM,
    TexteLibre,
    VraiFaux
}
