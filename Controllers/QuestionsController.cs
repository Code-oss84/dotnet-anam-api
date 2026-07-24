using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestionFormations.Data;
using GestionFormations.Models;

namespace GestionFormations.Controllers;

[Authorize(Roles = "Admin,Formateur")]
public class QuestionsController : Controller
{
    private readonly ApplicationDbContext _context;

    public QuestionsController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(int? contenuId)
    {
        var query = _context.Questions
            .Include(q => q.Contenu)
            .Include(q => q.ChoixReponses)
            .AsQueryable();
        if (contenuId.HasValue)
            query = query.Where(q => q.ContenuId == contenuId.Value);
        return View(await query.ToListAsync());
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();
        var question = await _context.Questions
            .Include(q => q.Contenu)
            .Include(q => q.ChoixReponses)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (question == null) return NotFound();
        return View(question);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(int? contenuId)
    {
        ViewBag.Contenus = await _context.Contenus.Include(c => c.Module).ToListAsync();
        var question = new Question { Type = TypeQuestion.QCM, Points = 1 };
        if (contenuId.HasValue) question.ContenuId = contenuId.Value;
        return View(question);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([Bind("Enonce,Type,Points,ContenuId")] Question question, List<string> choixTextes, List<bool> choixCorrects)
    {
        if (ModelState.IsValid)
        {
            _context.Add(question);
            await _context.SaveChangesAsync();

            if (question.Type == TypeQuestion.QCM && choixTextes != null)
            {
                for (int i = 0; i < choixTextes.Count; i++)
                {
                    if (!string.IsNullOrWhiteSpace(choixTextes[i]))
                    {
                        _context.ChoixReponses.Add(new ChoixReponse
                        {
                            Texte = choixTextes[i],
                            EstCorrect = i < choixCorrects.Count && choixCorrects[i],
                            QuestionId = question.Id
                        });
                    }
                }
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index), new { contenuId = question.ContenuId });
        }
        ViewBag.Contenus = await _context.Contenus.Include(c => c.Module).ToListAsync();
        return View(question);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var question = await _context.Questions.Include(q => q.ChoixReponses).FirstOrDefaultAsync(m => m.Id == id);
        if (question == null) return NotFound();
        ViewBag.Contenus = await _context.Contenus.Include(c => c.Module).ToListAsync();
        return View(question);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Enonce,Type,Points,ContenuId")] Question question, List<string> choixTextes, List<bool> choixCorrects)
    {
        if (id != question.Id) return NotFound();
        if (ModelState.IsValid)
        {
            var existingChoix = await _context.ChoixReponses.Where(c => c.QuestionId == id).ToListAsync();
            _context.ChoixReponses.RemoveRange(existingChoix);

            _context.Update(question);
            await _context.SaveChangesAsync();

            if (question.Type == TypeQuestion.QCM && choixTextes != null)
            {
                for (int i = 0; i < choixTextes.Count; i++)
                {
                    if (!string.IsNullOrWhiteSpace(choixTextes[i]))
                    {
                        _context.ChoixReponses.Add(new ChoixReponse
                        {
                            Texte = choixTextes[i],
                            EstCorrect = i < choixCorrects.Count && choixCorrects[i],
                            QuestionId = question.Id
                        });
                    }
                }
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index), new { contenuId = question.ContenuId });
        }
        ViewBag.Contenus = await _context.Contenus.Include(c => c.Module).ToListAsync();
        return View(question);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var question = await _context.Questions.Include(q => q.Contenu).FirstOrDefaultAsync(m => m.Id == id);
        if (question == null) return NotFound();
        return View(question);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var question = await _context.Questions.FindAsync(id);
        var contenuId = question?.ContenuId;
        if (question != null)
        {
            _context.Questions.Remove(question);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index), new { contenuId });
    }
}
