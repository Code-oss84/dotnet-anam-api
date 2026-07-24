using System.ComponentModel.DataAnnotations;

namespace GestionFormations.Models;

public enum StatutInscription
{
    EnAttente,
    Acceptee,
    Refusee,
    Terminee
}

public class Inscription
{
    public int Id { get; set; }

    [Display(Name = "Date d'inscription")]
    public DateTime DateInscription { get; set; } = DateTime.UtcNow;

    [Display(Name = "Statut")]
    public StatutInscription Statut { get; set; } = StatutInscription.EnAttente;

    [Display(Name = "Note finale")]
    [Range(0, 20)]
    public double? NoteFinale { get; set; }

    [Display(Name = "Progression (%)")]
    [Range(0, 100)]
    public double Progression { get; set; } = 0;

    [Display(Name = "Formation")]
    public int FormationId { get; set; }
    public Formation? Formation { get; set; }

    [Display(Name = "Apprenant")]
    public string? ApprenantId { get; set; }
    public Apprenant? Apprenant { get; set; }
}
