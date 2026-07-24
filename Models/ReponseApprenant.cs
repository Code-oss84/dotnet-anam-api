using System.ComponentModel.DataAnnotations;

namespace GestionFormations.Models;

public class ReponseApprenant
{
    public int Id { get; set; }

    [Display(Name = "Réponse textuelle")]
    public string? TexteReponse { get; set; }

    public bool? EstCorrect { get; set; }

    [Display(Name = "Points obtenus")]
    [Range(0, 100)]
    public int PointsObtenus { get; set; }

    [Display(Name = "Date de réponse")]
    public DateTime DateReponse { get; set; } = DateTime.UtcNow;

    [Display(Name = "Question")]
    public int QuestionId { get; set; }
    public Question? Question { get; set; }

    [Display(Name = "Choix sélectionné")]
    public int? ChoixReponseId { get; set; }
    public ChoixReponse? ChoixReponse { get; set; }

    [Display(Name = "Apprenant")]
    public string? ApprenantId { get; set; }
    public Apprenant? Apprenant { get; set; }
}
