using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestionFormations.Data;
using GestionFormations.Models;

namespace GestionFormations.Controllers;

[Authorize(Roles = "Admin")]
public class CategoriesController : Controller
{
    private readonly ApplicationDbContext _context;

    public CategoriesController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _context.Categories.ToListAsync());
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();
        var categorie = await _context.Categories.FirstOrDefaultAsync(m => m.Id == id);
        if (categorie == null) return NotFound();
        return View(categorie);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Nom,Description")] Categorie categorie)
    {
        if (ModelState.IsValid)
        {
            _context.Add(categorie);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(categorie);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var categorie = await _context.Categories.FindAsync(id);
        if (categorie == null) return NotFound();
        return View(categorie);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Nom,Description")] Categorie categorie)
    {
        if (id != categorie.Id) return NotFound();
        if (ModelState.IsValid)
        {
            _context.Update(categorie);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(categorie);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var categorie = await _context.Categories.FirstOrDefaultAsync(m => m.Id == id);
        if (categorie == null) return NotFound();
        return View(categorie);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var categorie = await _context.Categories.FindAsync(id);
        if (categorie != null)
        {
            _context.Categories.Remove(categorie);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}
