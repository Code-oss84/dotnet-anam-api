using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestionFormations.Data;
using GestionFormations.Models;

namespace GestionFormations.Controllers;

[Authorize(Roles = "Admin")]
public class FormateursController : Controller
{
    private readonly ApplicationDbContext _context;

    public FormateursController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _context.Formateurs.ToListAsync());
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();
        var formateur = await _context.Formateurs.FirstOrDefaultAsync(m => m.Id == id);
        if (formateur == null) return NotFound();
        return View(formateur);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Nom,Prenom,Email,Telephone,Specialite,PhotoUrl,Biographie,EstActif")] Formateur formateur)
    {
        if (ModelState.IsValid)
        {
            _context.Add(formateur);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(formateur);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var formateur = await _context.Formateurs.FindAsync(id);
        if (formateur == null) return NotFound();
        return View(formateur);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Nom,Prenom,Email,Telephone,Specialite,PhotoUrl,Biographie,EstActif")] Formateur formateur)
    {
        if (id != formateur.Id) return NotFound();
        if (ModelState.IsValid)
        {
            _context.Update(formateur);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(formateur);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var formateur = await _context.Formateurs.FirstOrDefaultAsync(m => m.Id == id);
        if (formateur == null) return NotFound();
        return View(formateur);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var formateur = await _context.Formateurs.FindAsync(id);
        if (formateur != null)
        {
            _context.Formateurs.Remove(formateur);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}
