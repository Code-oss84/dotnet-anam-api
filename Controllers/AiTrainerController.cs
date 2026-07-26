using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json;
using GestionFormations.Data;
using GestionFormations.Models;

namespace GestionFormations.Controllers;

[Authorize]
public class AiTrainerController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;

    public AiTrainerController(
        ApplicationDbContext context,
        IConfiguration configuration,
        IHttpClientFactory httpClientFactory)
    {
        _context = context;
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<IActionResult> Index(int? formationId)
    {
        var formations = await _context.Formations
            .Include(f => f.Categorie)
            .ToListAsync();

        ViewBag.Formations = formations;
        ViewBag.FormationId = formationId;

        if (formationId.HasValue)
        {
            var modules = await _context.Modules
                .Include(m => m.Contenus)
                .Where(m => m.FormationId == formationId.Value)
                .OrderBy(m => m.Ordre)
                .ToListAsync();
            ViewBag.Modules = modules;
        }

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> GetModuleContext(int moduleId)
    {
        var module = await _context.Modules
            .Include(m => m.Formation)
            .Include(m => m.Contenus).ThenInclude(c => c.Questions)
            .FirstOrDefaultAsync(m => m.Id == moduleId);

        if (module == null) return NotFound();

        var contenus = module.Contenus.Select(c => new
        {
            c.Titre,
            c.Type,
            c.Texte
        }).ToList();

        var contextData = new
        {
            module.Titre,
            module.Description,
            Formation = module.Formation?.Titre,
            Contenus = contenus
        };

        return Json(contextData);
    }

    [HttpPost]
    public async Task<IActionResult> CreateSessionToken([FromBody] SessionTokenRequest request)
    {
        var anamConfig = _configuration.GetSection("AnamAi");
        var apiKey = anamConfig["ApiKey"];
        var personaId = anamConfig["PersonaId"];

        if (string.IsNullOrEmpty(apiKey))
            return BadRequest(new { error = "Clé API Anam.ai non configurée" });

        var client = _httpClientFactory.CreateClient();

        var body = new Dictionary<string, object>();

        if (!string.IsNullOrEmpty(personaId))
        {
            body["personaConfig"] = new Dictionary<string, string>
            {
                ["personaId"] = personaId
            };
        }
        else
        {
            return BadRequest(new { error = "PersonaId non configuré dans appsettings.json" });
        }

        var json = JsonSerializer.Serialize(body);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var httpRequest = new HttpRequestMessage(HttpMethod.Post, "https://api.anam.ai/v1/auth/session-token");
        httpRequest.Headers.Add("Authorization", $"Bearer {apiKey}");
        httpRequest.Content = content;

        var response = await client.SendAsync(httpRequest);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            return BadRequest(new { error = $"Erreur Anam API: {response.StatusCode}", details = errorBody });
        }

        var responseBody = await response.Content.ReadAsStringAsync();
        var tokenData = JsonSerializer.Deserialize<JsonElement>(responseBody);

        return Json(new { sessionToken = tokenData.GetProperty("sessionToken").GetString() });
    }
}

public class SessionTokenRequest
{
    public string? SystemPrompt { get; set; }
}
