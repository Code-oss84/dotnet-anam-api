using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestionFormations.Data;
using GestionFormations.Models;

namespace GestionFormations.Controllers;

[Authorize]
public class QuizController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<Apprenant> _userManager;

    public QuizController(ApplicationDbContext context, UserManager<Apprenant> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Passer(int? contenuId)
    {
        if (contenuId == null) return NotFound();

        var contenu = await _context.Contenus
            .Include(c => c.Module).ThenInclude(m => m.Formation)
            .Include(c => c.Questions).ThenInclude(q => q.ChoixReponses)
            .FirstOrDefaultAsync(c => c.Id == contenuId);

        if (contenu == null) return NotFound();
        if (contenu.Type != TypeContenu.Examen && contenu.Type != TypeContenu.Quiz)
            return RedirectToAction("Details", "Contenus", new { id = contenuId });

        return View(contenu);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Soumettre(int contenuId, Dictionary<int, int> reponses)
    {
        var userId = _userManager.GetUserId(User);
        if (userId == null) return Challenge();

        var contenu = await _context.Contenus
            .Include(c => c.Questions).ThenInclude(q => q.ChoixReponses)
            .FirstOrDefaultAsync(c => c.Id == contenuId);

        if (contenu == null) return NotFound();

        int totalPoints = 0;
        int obtainedPoints = 0;

        foreach (var question in contenu.Questions)
        {
            totalPoints += question.Points;

            if (reponses.TryGetValue(question.Id, out int choixId))
            {
                var choix = question.ChoixReponses.FirstOrDefault(c => c.Id == choixId);
                bool isCorrect = choix?.EstCorrect ?? false;
                int points = isCorrect ? question.Points : 0;
                obtainedPoints += points;

                _context.ReponsesApprenants.Add(new ReponseApprenant
                {
                    QuestionId = question.Id,
                    ChoixReponseId = choixId,
                    ApprenantId = userId,
                    EstCorrect = isCorrect,
                    PointsObtenus = points,
                    DateReponse = DateTime.UtcNow
                });
            }
        }

        await _context.SaveChangesAsync();

        double note = totalPoints > 0 ? (double)obtainedPoints / totalPoints * 20 : 0;
        ViewBag.Note = Math.Round(note, 2);
        ViewBag.Total = totalPoints;
        ViewBag.Obtained = obtainedPoints;

        return View("Resultat");
    }
}
