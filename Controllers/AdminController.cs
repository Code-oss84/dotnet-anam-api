using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestionFormations.Data;
using GestionFormations.Models;

namespace GestionFormations.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;

    public AdminController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Dashboard()
    {
        var stats = new AdminDashboardViewModel
        {
            TotalFormations = await _context.Formations.CountAsync(),
            TotalFormateurs = await _context.Formateurs.CountAsync(),
            TotalApprenants = await _context.Inscriptions.Select(i => i.ApprenantId).Distinct().CountAsync(),
            TotalCategories = await _context.Categories.CountAsync(),
            TotalModules = await _context.Modules.CountAsync(),
            TotalContenus = await _context.Contenus.CountAsync(),
            InscriptionsEnAttente = await _context.Inscriptions.CountAsync(i => i.Statut == StatutInscription.EnAttente),
            InscriptionsAcceptees = await _context.Inscriptions.CountAsync(i => i.Statut == StatutInscription.Acceptee),
            DernieresInscriptions = await _context.Inscriptions
                .Include(i => i.Formation)
                .Include(i => i.Apprenant)
                .OrderByDescending(i => i.DateInscription)
                .Take(10)
                .ToListAsync(),
            FormationsPopulaires = await _context.Formations
                .Include(f => f.Categorie)
                .Include(f => f.Inscriptions)
                .OrderByDescending(f => f.Inscriptions.Count)
                .Take(5)
                .ToListAsync()
        };

        return View(stats);
    }
}
