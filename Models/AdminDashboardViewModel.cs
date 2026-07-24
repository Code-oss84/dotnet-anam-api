namespace GestionFormations.Models;

public class AdminDashboardViewModel
{
    public int TotalFormations { get; set; }
    public int TotalFormateurs { get; set; }
    public int TotalApprenants { get; set; }
    public int TotalCategories { get; set; }
    public int TotalModules { get; set; }
    public int TotalContenus { get; set; }
    public int InscriptionsEnAttente { get; set; }
    public int InscriptionsAcceptees { get; set; }
    public List<Inscription> DernieresInscriptions { get; set; } = new();
    public List<Formation> FormationsPopulaires { get; set; } = new();
}
