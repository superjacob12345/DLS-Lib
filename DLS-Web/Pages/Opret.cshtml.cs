using DLS_Lib.Repository;
using DLS_Lib.UserStories;
using System.ComponentModel.DataAnnotations;
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

        // Samme regler som på Spejder-klassen
        [BindProperty]
        [Required(ErrorMessage = "Skriv dit navn")]
        [StringLength(100, ErrorMessage = "Navnet må højst være 100 tegn")]
        public string Navn { get; set; } = "";

        [BindProperty]
        [Required(ErrorMessage = "Skriv et brugernavn")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Brugernavnet skal være mellem 3 og 50 tegn")]
        [RegularExpression(@"^\S+$", ErrorMessage = "Brugernavnet må ikke indeholde mellemrum")]
        public string BrugerNavn { get; set; } = "";

        [BindProperty]
        [Required(ErrorMessage = "Skriv en adgangskode")]
        [StringLength(100, MinimumLength = 4, ErrorMessage = "Adgangskoden skal være mellem 4 og 100 tegn")]
        public string Adgangskode { get; set; } = "";

        [BindProperty]
        [Required(ErrorMessage = "Gentag adgangskoden")]
        [Compare(nameof(Adgangskode), ErrorMessage = "De to adgangskoder er ikke ens")]
        public string GentagAdgangskode { get; set; } = "";

        [BindProperty]
        [Display(Name = "Fødselsdato")]
        [Foedselsdato]
        public DateOnly Foedselsdato { get; set; }

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

            // Reglerne på felterne ovenfor er allerede tjekket af ASP.NET.
            // Brugernavnet må desuden ikke være taget.
            if (BrugerNavn != "" && BrugerNavnOptaget())
            {
                ModelState.AddModelError(nameof(BrugerNavn), "Brugernavnet \"" + BrugerNavn + "\" er allerede taget");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Opret spejderen og log den ind med det samme
            Spejder spejder = new Spejder(Navn, BrugerNavn, Adgangskode, Foedselsdato);
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
