using DLS_Lib.Repository;
using DLS_Lib.UserStories;
using DLS_Web.Hjaelpere;
using Microsoft.AspNetCore.Mvc;

namespace DLS_Web.Pages.Ledere
{
    // Bruges både til at oprette (uden id) og rette (med id)
    public class RedigerModel : LederSide
    {
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
            Leder.Navn = Leder.Navn.Trim();
            Leder.BrugerNavn = Leder.BrugerNavn.Trim();

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
            else if (Bruger.BrugerNavnOptaget(Leder.BrugerNavn, _spejderRepo, _repo, 0, Leder.Id))
            {
                ModelState.AddModelError("", "Brugernavnet \"" + Leder.BrugerNavn + "\" er allerede taget");
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
