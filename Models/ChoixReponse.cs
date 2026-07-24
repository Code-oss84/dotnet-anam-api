using System.ComponentModel.DataAnnotations;

namespace GestionFormations.Models;

public class ChoixReponse
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Le texte est requis")]
    [StringLength(500)]
    public string Texte { get; set; } = string.Empty;

    public bool EstCorrect { get; set; }

    [Display(Name = "Question")]
    public int QuestionId { get; set; }
    public Question? Question { get; set; }
}
