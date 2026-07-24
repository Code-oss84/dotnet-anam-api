using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestionFormations.Data;
using GestionFormations.Models;

namespace GestionFormations.Controllers;

[Authorize(Roles = "Admin,Formateur")]
public class ContenusController : Controller
{
    private readonly ApplicationDbContext _context;

    public ContenusController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(int? moduleId)
    {
        var query = _context.Contenus
            .Include(c => c.Module)
            .ThenInclude(m => m.Formation)
            .AsQueryable();
        if (moduleId.HasValue)
        {
            query = query.Where(c => c.ModuleId == moduleId.Value);
        }
        return View(await query.OrderBy(c => c.Ordre).ToListAsync());
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();
        var contenu = await _context.Contenus
            .Include(c => c.Module)
            .Include(c => c.Questions).ThenInclude(q => q.ChoixReponses)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (contenu == null) return NotFound();
        return View(contenu);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(int? moduleId)
    {
        ViewBag.Modules = await _context.Modules.Include(m => m.Formation).ToListAsync();
        var contenu = new Contenu { Type = TypeContenu.Cours };
        if (moduleId.HasValue) contenu.ModuleId = moduleId.Value;
        return View(contenu);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([Bind("Titre,Description,Type,Texte,VideoUrl,DocumentUrl,DureeMinutes,Ordre,ModuleId")] Contenu contenu)
    {
        if (ModelState.IsValid)
        {
            _context.Add(contenu);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { moduleId = contenu.ModuleId });
        }
        ViewBag.Modules = await _context.Modules.Include(m => m.Formation).ToListAsync();
        return View(contenu);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var contenu = await _context.Contenus.FindAsync(id);
        if (contenu == null) return NotFound();
        ViewBag.Modules = await _context.Modules.Include(m => m.Formation).ToListAsync();
        return View(contenu);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Titre,Description,Type,Texte,VideoUrl,DocumentUrl,DureeMinutes,Ordre,ModuleId")] Contenu contenu)
    {
        if (id != contenu.Id) return NotFound();
        if (ModelState.IsValid)
        {
            _context.Update(contenu);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { moduleId = contenu.ModuleId });
        }
        ViewBag.Modules = await _context.Modules.Include(m => m.Formation).ToListAsync();
        return View(contenu);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var contenu = await _context.Contenus.Include(c => c.Module).FirstOrDefaultAsync(m => m.Id == id);
        if (contenu == null) return NotFound();
        return View(contenu);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var contenu = await _context.Contenus.FindAsync(id);
        var moduleId = contenu?.ModuleId;
        if (contenu != null)
        {
            _context.Contenus.Remove(contenu);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index), new { moduleId });
    }
}
