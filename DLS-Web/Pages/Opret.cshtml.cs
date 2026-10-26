using DLS_Lib.Repository;
using DLS_Lib.UserStories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DLS_Web.Pages
{
    // Her kan man selv oprette sig som spejder.
    // Nye ledere kan kun oprettes af en leder (under Ledere).
    public class OpretModel : PageModel
    {
        private SpejderRepository _spejderRepo;
        private LederRepository _lederRepo;

        [BindProperty]
        public string Navn { get; set; } = "";

        [BindProperty]
        public string BrugerNavn { get; set; } = "";

        [BindProperty]
        public string Adgangskode { get; set; } = "";

        [BindProperty]
        public string GentagAdgangskode { get; set; } = "";

        [BindProperty]
        public DateOnly Foedselsdato { get; set; }

        // Fejlbeskeder der vises over formularen
        public List<string> Fejl { get; set; } = new List<string>();

        public OpretModel(SpejderRepository spejderRepo, LederRepository lederRepo)
        {
            _spejderRepo = spejderRepo;
            _lederRepo = lederRepo;
        }

        public IActionResult OnGet()
        {
            // Er man allerede logget ind, behøver man ikke oprette sig
            if (HttpContext.Session.GetString("Rolle") != null)
            {
                return RedirectToPage("/Aktiviteter/Index");
            }

            Foedselsdato = DateOnly.FromDateTime(DateTime.Today.AddYears(-10));
            return Page();
        }

        public IActionResult OnPost()
        {
            // Tomme tekstfelter kommer ind som null, så de laves om til ""
            if (Navn == null)
            {
                Navn = "";
            }
            if (BrugerNavn == null)
            {
                BrugerNavn = "";
            }
            if (Adgangskode == null)
            {
                Adgangskode = "";
            }
            if (GentagAdgangskode == null)
            {
                GentagAdgangskode = "";
            }
            Navn = Navn.Trim();
            BrugerNavn = BrugerNavn.Trim();

            // Tjek at felterne er udfyldt rigtigt
            if (Navn == "")
            {
                Fejl.Add("Skriv dit navn");
            }
            if (BrugerNavn == "")
            {
                Fejl.Add("Skriv et brugernavn");
            }
            else if (BrugerNavnOptaget())
            {
                Fejl.Add("Brugernavnet \"" + BrugerNavn + "\" er allerede taget");
            }
            if (Adgangskode.Length < 4)
            {
                Fejl.Add("Adgangskoden skal være mindst 4 tegn");
            }
            else if (Adgangskode != GentagAdgangskode)
            {
                Fejl.Add("De to adgangskoder er ikke ens");
            }
            if (Foedselsdato > DateOnly.FromDateTime(DateTime.Today))
            {
                Fejl.Add("Fødselsdatoen kan ikke ligge i fremtiden");
            }

            if (Fejl.Count > 0)
            {
                return Page();
            }

            // Opret spejderen og log den ind med det samme
            Spejder spejder = new Spejder(0, Navn, BrugerNavn, Adgangskode, Foedselsdato);
            _spejderRepo.Opret(spejder);
            HttpContext.Session.SetInt32("BrugerId", spejder.Id);
            HttpContext.Session.SetString("Navn", spejder.Navn);
            HttpContext.Session.SetString("Rolle", "Spejder");

            TempData["Besked"] = "Velkommen, " + Navn + ". Din bruger er oprettet";
            return RedirectToPage("/Aktiviteter/Index");
        }

        // Brugernavnet må ikke bruges af en anden spejder eller leder
        private bool BrugerNavnOptaget()
        {
            string navn = BrugerNavn.ToLower();
            foreach (Spejder s in _spejderRepo.HentAlle())
            {
                if (s.BrugerNavn.ToLower() == navn)
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
