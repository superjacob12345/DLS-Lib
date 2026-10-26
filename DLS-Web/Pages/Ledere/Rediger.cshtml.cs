using DLS_Lib.Repository;
using DLS_Lib.UserStories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DLS_Web.Pages.Ledere
{
    // Bruges både til at oprette (uden id) og rette (med id)
    public class RedigerModel : PageModel
    {
        private LederRepository _repo;
        private SpejderRepository _spejderRepo;

        [BindProperty]
        public Leder Leder { get; set; } = new Leder();

        // Fejlbeskeder der vises over formularen
        public List<string> Fejl { get; set; } = new List<string>();

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
            // Kun ledere må oprette og rette ledere
            if (HttpContext.Session.GetString("Rolle") != "Leder")
            {
                return RedirectToPage("/Login");
            }

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
            if (HttpContext.Session.GetString("Rolle") != "Leder")
            {
                return RedirectToPage("/Login");
            }

            // Tomme tekstfelter kommer ind som null, så de laves om til ""
            if (Leder.Navn == null)
            {
                Leder.Navn = "";
            }
            if (Leder.BrugerNavn == null)
            {
                Leder.BrugerNavn = "";
            }
            if (Leder.Adgangskode == null)
            {
                Leder.Adgangskode = "";
            }
            Leder.Navn = Leder.Navn.Trim();
            Leder.BrugerNavn = Leder.BrugerNavn.Trim();

            // Lader man adgangskoden stå tom ved rettelse, beholdes den gamle
            Leder? gammel = _repo.HentEn(Leder.Id);
            if (gammel != null && Leder.Adgangskode == "")
            {
                Leder.Adgangskode = gammel.Adgangskode;
            }

            // Tjek at felterne er udfyldt rigtigt
            if (Leder.Navn == "")
            {
                Fejl.Add("Skriv lederens navn");
            }
            if (Leder.BrugerNavn == "")
            {
                Fejl.Add("Skriv et brugernavn");
            }
            else if (BrugerNavnOptaget())
            {
                Fejl.Add("Brugernavnet \"" + Leder.BrugerNavn + "\" er allerede taget");
            }
            if (Leder.Adgangskode == "")
            {
                Fejl.Add("Skriv en adgangskode");
            }

            if (Fejl.Count > 0)
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

        // Brugernavnet må ikke bruges af en anden leder eller en spejder
        private bool BrugerNavnOptaget()
        {
            string navn = Leder.BrugerNavn.ToLower();
            foreach (Leder l in _repo.HentAlle())
            {
                if (l.Id != Leder.Id && l.BrugerNavn.ToLower() == navn)
                {
                    return true;
                }
            }
            foreach (Spejder s in _spejderRepo.HentAlle())
            {
                if (s.BrugerNavn.ToLower() == navn)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
