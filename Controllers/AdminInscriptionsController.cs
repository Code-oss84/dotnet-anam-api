using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestionFormations.Data;
using GestionFormations.Models;

namespace GestionFormations.Controllers;

[Authorize(Roles = "Admin")]
public class AdminInscriptionsController : Controller
{
    private readonly ApplicationDbContext _context;

    public AdminInscriptionsController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(string? statut)
    {
        var query = _context.Inscriptions
            .Include(i => i.Formation)
            .Include(i => i.Apprenant)
            .AsQueryable();

        if (!string.IsNullOrEmpty(statut) && Enum.TryParse<StatutInscription>(statut, out var s))
        {
            query = query.Where(i => i.Statut == s);
        }

        return View(await query.OrderByDescending(i => i.DateInscription).ToListAsync());
    }

    public async Task<IActionResult> Traiter(int? id, StatutInscription statut)
    {
        if (id == null) return NotFound();
        var inscription = await _context.Inscriptions.FindAsync(id);
        if (inscription == null) return NotFound();

        inscription.Statut = statut;
        await _context.SaveChangesAsync();

        TempData["Success"] = $"Inscription {(statut == StatutInscription.Acceptee ? "acceptée" : "refusée")} avec succès.";
        return RedirectToAction(nameof(Index));
    }
}
