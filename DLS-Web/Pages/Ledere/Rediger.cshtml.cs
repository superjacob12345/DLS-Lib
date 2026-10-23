using DLS_Lib.Repository;
using DLS_Lib.UserStories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DLS_Web.Pages.Ledere
{
    // Bruges både til at oprette (uden id) og rette (med id)
    public class RedigerModel : PageModel
    {
        // Kun ledere har adgang til siden. Kører før OnGet/OnPost.
        public override void OnPageHandlerExecuting(PageHandlerExecutingContext context)
        {
            string? rolle = HttpContext.Session.GetString("Rolle");
            if (rolle == null)
            {
                TempData["Fejl"] = "Du skal logge ind først";
                context.Result = RedirectToPage("/Login");
            }
            else if (rolle != "Leder")
            {
                TempData["Fejl"] = "Den side er kun for ledere";
                context.Result = RedirectToPage("/Index");
            }
        }

        private LederRepository _repo;
        private SpejderRepository _spejderRepo;

        [BindProperty]
        public Leder Leder { get; set; } = new Leder();

        public bool ErNy
        {
            get { return Leder.Id == 0; }
        }

        public RedigerModel(LederRepository repo, SpejderRepository spejderRepo)
        {
            _repo = repo;
            _spejderRepo = spejderRepo;
        }

        public IActionResult OnGet(int? id)
        {
            if (id != null)
            {
                Leder? leder = _repo.HentEn(id.Value);
                if (leder == null)
                {
                    return NotFound();
                }
                Leder = leder;
            }
            return Page();
        }

        public IActionResult OnPost()
        {
            // Tomme felter kommer ind som null, så de laves om til ""
            Leder.Navn = (Leder.Navn ?? "").Trim();
            Leder.BrugerNavn = (Leder.BrugerNavn ?? "").Trim();
            Leder.Adgangskode = Leder.Adgangskode ?? "";

            // Lader man adgangskoden stå tom ved rettelse, beholdes den gamle
            Leder? gammel = _repo.HentEn(Leder.Id);
            if (gammel != null && Leder.Adgangskode == "")
            {
                Leder.Adgangskode = gammel.Adgangskode;
            }

            if (Leder.Navn == "")
            {
                ModelState.AddModelError("", "Skriv lederens navn");
            }
            if (Leder.BrugerNavn == "")
            {
                ModelState.AddModelError("", "Skriv et brugernavn");
            }
            else
            {
                // Brugernavnet må ikke bruges af en anden leder eller en spejder
                bool optaget = _spejderRepo.HentAlle().Any(s => s.BrugerNavn.ToLower() == Leder.BrugerNavn.ToLower())
                    || _repo.HentAlle().Any(l => l.Id != Leder.Id && l.BrugerNavn.ToLower() == Leder.BrugerNavn.ToLower());
                if (optaget)
                {
                    ModelState.AddModelError("", "Brugernavnet \"" + Leder.BrugerNavn + "\" er allerede taget");
                }
            }
            if (Leder.Adgangskode == "")
            {
                ModelState.AddModelError("", "Skriv en adgangskode");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (Leder.Id == 0)
            {
                _repo.Opret(Leder);
                TempData["Besked"] = Leder.Navn + " er oprettet";
            }
            else
            {
                _repo.Opdater(Leder);
                TempData["Besked"] = Leder.Navn + " er gemt";
            }
            return RedirectToPage("Index");
        }
    }
}
