using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestionFormations.Data;
using GestionFormations.Models;

namespace GestionFormations.Controllers;

[Authorize(Roles = "Admin,Formateur")]
public class ModulesController : Controller
{
    private readonly ApplicationDbContext _context;

    public ModulesController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(int? formationId)
    {
        var query = _context.Modules.Include(m => m.Formation).AsQueryable();
        if (formationId.HasValue)
        {
            query = query.Where(m => m.FormationId == formationId.Value);
        }
        return View(await query.OrderBy(m => m.Ordre).ToListAsync());
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();
        var module = await _context.Modules
            .Include(m => m.Formation)
            .Include(m => m.Contenus)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (module == null) return NotFound();
        return View(module);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(int? formationId)
    {
        ViewBag.Formations = await _context.Formations.ToListAsync();
        var module = new Module();
        if (formationId.HasValue) module.FormationId = formationId.Value;
        return View(module);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([Bind("Titre,Description,Ordre,DureeHeures,FormationId")] Module module)
    {
        if (ModelState.IsValid)
        {
            _context.Add(module);
            await _context.SaveChangesAsync();
            return RedirectToAction("Edit", "Formations", new { id = module.FormationId });
        }
        ViewBag.Formations = await _context.Formations.ToListAsync();
        return View(module);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var module = await _context.Modules.FindAsync(id);
        if (module == null) return NotFound();
        ViewBag.Formations = await _context.Formations.ToListAsync();
        return View(module);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Titre,Description,Ordre,DureeHeures,FormationId")] Module module)
    {
        if (id != module.Id) return NotFound();
        if (ModelState.IsValid)
        {
            _context.Update(module);
            await _context.SaveChangesAsync();
            return RedirectToAction("Edit", "Formations", new { id = module.FormationId });
        }
        ViewBag.Formations = await _context.Formations.ToListAsync();
        return View(module);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var module = await _context.Modules.Include(m => m.Formation).FirstOrDefaultAsync(m => m.Id == id);
        if (module == null) return NotFound();
        return View(module);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var module = await _context.Modules.FindAsync(id);
        var formationId = module?.FormationId;
        if (module != null)
        {
            _context.Modules.Remove(module);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction("Edit", "Formations", new { id = formationId });
    }
}
