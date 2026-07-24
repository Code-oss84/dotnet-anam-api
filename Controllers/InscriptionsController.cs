using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestionFormations.Data;
using GestionFormations.Models;

namespace GestionFormations.Controllers;

[Authorize]
public class InscriptionsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<Apprenant> _userManager;

    public InscriptionsController(ApplicationDbContext context, UserManager<Apprenant> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User);
        var isAdmin = User.IsInRole("Admin");

        var query = _context.Inscriptions
            .Include(i => i.Formation).ThenInclude(f => f.Categorie)
            .Include(i => i.Formation).ThenInclude(f => f.Formateur)
            .Include(i => i.Apprenant)
            .AsQueryable();

        if (!isAdmin)
        {
            query = query.Where(i => i.ApprenantId == userId);
        }

        return View(await query.OrderByDescending(i => i.DateInscription).ToListAsync());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SInscrire(int formationId)
    {
        var userId = _userManager.GetUserId(User);
        if (userId == null) return Challenge();

        var formation = await _context.Formations.FindAsync(formationId);
        if (formation == null) return NotFound();

        var existing = await _context.Inscriptions
            .FirstOrDefaultAsync(i => i.FormationId == formationId && i.ApprenantId == userId);
        if (existing != null)
        {
            TempData["Error"] = "Vous êtes déjà inscrit à cette formation.";
            return RedirectToAction("Details", "Formations", new { id = formationId });
        }

        var currentCount = await _context.Inscriptions.CountAsync(i => i.FormationId == formationId);
        if (currentCount >= formation.NombrePlaces)
        {
            TempData["Error"] = "Nombre de places atteint.";
            return RedirectToAction("Details", "Formations", new { id = formationId });
        }

        var inscription = new Inscription
        {
            FormationId = formationId,
            ApprenantId = userId,
            DateInscription = DateTime.UtcNow,
            Statut = StatutInscription.EnAttente
        };

        _context.Inscriptions.Add(inscription);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Inscription réussie !";
        return RedirectToAction("Details", "Formations", new { id = formationId });
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Traiter(int? id, StatutInscription statut)
    {
        if (id == null) return NotFound();
        var inscription = await _context.Inscriptions.FindAsync(id);
        if (inscription == null) return NotFound();

        inscription.Statut = statut;
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}
