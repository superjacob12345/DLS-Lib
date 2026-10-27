using DLS_Lib.Repository;
using DLS_Lib.UserStories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DLS_Web.Pages
{
    public class LoginModel : PageModel
    {
        private SpejderRepository _spejderRepo;
        private LederRepository _lederRepo;

        [BindProperty]
        public string BrugerNavn { get; set; } = "";

        [BindProperty]
        public string Adgangskode { get; set; } = "";

        public string Fejl { get; set; } = "";

        public LoginModel(SpejderRepository spejderRepo, LederRepository lederRepo)
        {
            _spejderRepo = spejderRepo;
            _lederRepo = lederRepo;
        }

        public IActionResult OnGet()
        {
            // Er man allerede logget ind, er der ingen grund til at vise login
            if (HttpContext.Session.GetString("Rolle") != null)
            {
                return RedirectToPage("/Aktiviteter/Index");
            }
            return Page();
        }

        public IActionResult OnPost()
        {
            // Først tjekkes om det er en leder, og ellers om det er en spejder
            foreach (Leder l in _lederRepo.HentAlle())
            {
                if (l.BrugerNavn == BrugerNavn && l.Adgangskode == Adgangskode)
                {
                    HttpContext.Session.SetInt32("BrugerId", l.Id);
                    HttpContext.Session.SetString("Navn", l.Navn);
                    HttpContext.Session.SetString("Rolle", "Leder");
                    TempData["Besked"] = "Velkommen, " + l.Navn;
                    return RedirectToPage("/Aktiviteter/Index");
                }
            }

            foreach (Spejder s in _spejderRepo.HentAlle())
            {
                if (s.BrugerNavn == BrugerNavn && s.Adgangskode == Adgangskode)
                {
                    HttpContext.Session.SetInt32("BrugerId", s.Id);
                    HttpContext.Session.SetString("Navn", s.Navn);
                    HttpContext.Session.SetString("Rolle", "Spejder");
                    TempData["Besked"] = "Velkommen, " + s.Navn;
                    return RedirectToPage("/Aktiviteter/Index");
                }
            }

            Fejl = "Forkert brugernavn eller adgangskode";
            return Page();
        }
    }
}
