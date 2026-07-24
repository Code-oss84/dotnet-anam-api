using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestionFormations.Data;
using GestionFormations.Models;

namespace GestionFormations.Controllers;

[Authorize(Roles = "Admin,Formateur")]
public class FormationsController : Controller
{
    private readonly ApplicationDbContext _context;

    public FormationsController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var formations = _context.Formations
            .Include(f => f.Categorie)
            .Include(f => f.Formateur);
        return View(await formations.ToListAsync());
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();
        var formation = await _context.Formations
            .Include(f => f.Categorie)
            .Include(f => f.Formateur)
            .Include(f => f.Modules).ThenInclude(m => m.Contenus)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (formation == null) return NotFound();
        return View(formation);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create()
    {
        ViewBag.Categories = await _context.Categories.ToListAsync();
        ViewBag.Formateurs = await _context.Formateurs.Where(f => f.EstActif).ToListAsync();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([Bind("Titre,Description,DureeHeures,Prix,DateDebut,DateFin,NombrePlaces,ImageUrl,EstActive,CategorieId,FormateurId")] Formation formation)
    {
        if (ModelState.IsValid)
        {
            _context.Add(formation);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        ViewBag.Categories = await _context.Categories.ToListAsync();
        ViewBag.Formateurs = await _context.Formateurs.Where(f => f.EstActif).ToListAsync();
        return View(formation);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var formation = await _context.Formations
            .Include(f => f.Modules.OrderBy(m => m.Ordre))
            .FirstOrDefaultAsync(f => f.Id == id);
        if (formation == null) return NotFound();
        ViewBag.Categories = await _context.Categories.ToListAsync();
        ViewBag.Formateurs = await _context.Formateurs.Where(f => f.EstActif).ToListAsync();
        return View(formation);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Titre,Description,DureeHeures,Prix,DateDebut,DateFin,NombrePlaces,ImageUrl,EstActive,CategorieId,FormateurId")] Formation formation)
    {
        if (id != formation.Id) return NotFound();
        if (ModelState.IsValid)
        {
            _context.Update(formation);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        ViewBag.Categories = await _context.Categories.ToListAsync();
        ViewBag.Formateurs = await _context.Formateurs.Where(f => f.EstActif).ToListAsync();
        return View(formation);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var formation = await _context.Formations
            .Include(f => f.Categorie)
            .Include(f => f.Formateur)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (formation == null) return NotFound();
        return View(formation);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var formation = await _context.Formations.FindAsync(id);
        if (formation != null)
        {
            _context.Formations.Remove(formation);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}
