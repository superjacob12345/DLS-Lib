using DLS_Lib.Repository;
using DLS_Lib.UserStories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DLS_Web.Pages.Spejdere
{
    // Bruges både til at oprette (uden id) og rette (med id)
    public class RedigerModel : PageModel
    {
        private SpejderRepository _repo;
        private LederRepository _lederRepo;
        private TegnRepository _tegnRepo;

        [BindProperty]
        public Spejder Spejder { get; set; } = new Spejder();

        public List<Tegn> AlleTegn { get; set; } = new List<Tegn>();

        // Fejlbeskeder der vises over formularen
        public List<string> Fejl { get; set; } = new List<string>();

        public bool ErNy
        {
            get { return Spejder.Id == 0; }
        }

        public RedigerModel(SpejderRepository repo, LederRepository lederRepo, TegnRepository tegnRepo)
        {
            _repo = repo;
            _lederRepo = lederRepo;
            _tegnRepo = tegnRepo;
        }

        public IActionResult OnGet(int? id)
        {
            // Kun ledere må oprette og rette spejdere
            if (HttpContext.Session.GetString("Rolle") != "Leder")
            {
                return RedirectToPage("/Login");
            }

            if (id != null)
            {
                Spejder? spejder = _repo.HentEn(id.Value);
                if (spejder == null)
                {
                    return NotFound();
                }
                Spejder = spejder;
            }
            else
            {
                Spejder.Foeselsdag = DateOnly.FromDateTime(DateTime.Today.AddYears(-10));
            }

            AlleTegn = _tegnRepo.HentAlle();
            return Page();
        }

        public IActionResult OnPost()
        {
            if (HttpContext.Session.GetString("Rolle") != "Leder")
            {
                return RedirectToPage("/Login");
            }

            // Tomme tekstfelter kommer ind som null, så de laves om til ""
            if (Spejder.Navn == null)
            {
                Spejder.Navn = "";
            }
            if (Spejder.BrugerNavn == null)
            {
                Spejder.BrugerNavn = "";
            }
            if (Spejder.Adgangskode == null)
            {
                Spejder.Adgangskode = "";
            }
            Spejder.Navn = Spejder.Navn.Trim();
            Spejder.BrugerNavn = Spejder.BrugerNavn.Trim();

            // Lader man adgangskoden stå tom ved rettelse, beholdes den gamle
            Spejder? gammel = _repo.HentEn(Spejder.Id);
            if (gammel != null && Spejder.Adgangskode == "")
            {
                Spejder.Adgangskode = gammel.Adgangskode;
            }

            // Tjek at felterne er udfyldt rigtigt
            if (Spejder.Navn == "")
            {
                Fejl.Add("Skriv spejderens navn");
            }
            if (Spejder.BrugerNavn == "")
            {
                Fejl.Add("Skriv et brugernavn");
            }
            else if (BrugerNavnOptaget())
            {
                Fejl.Add("Brugernavnet \"" + Spejder.BrugerNavn + "\" er allerede taget");
            }
            if (Spejder.Adgangskode == "")
            {
                Fejl.Add("Skriv en adgangskode");
            }
            if (Spejder.Foeselsdag > DateOnly.FromDateTime(DateTime.Today))
            {
                Fejl.Add("Fødselsdatoen kan ikke ligge i fremtiden");
            }

            if (Fejl.Count > 0)
            {
                AlleTegn = _tegnRepo.HentAlle();
                return Page();
            }

            if (Spejder.Id == 0)
            {
                _repo.Opret(Spejder);
                TempData["Besked"] = Spejder.Navn + " er oprettet";
            }
            else
            {
                _repo.Opdater(Spejder);
                TempData["Besked"] = Spejder.Navn + " er gemt";
            }
            return RedirectToPage("Index");
        }

        // Brugernavnet må ikke bruges af en anden spejder eller en leder
        private bool BrugerNavnOptaget()
        {
            string navn = Spejder.BrugerNavn.ToLower();
            foreach (Spejder s in _repo.HentAlle())
            {
                if (s.Id != Spejder.Id && s.BrugerNavn.ToLower() == navn)
                {
                    return true;
                }
            }
            foreach (Leder l in _lederRepo.HentAlle())
            {
                if (l.BrugerNavn.ToLower() == navn)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
