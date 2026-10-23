using DLS_Lib.Repository;
using DLS_Lib.UserStories;
using DLS_Web.Hjaelpere;
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
            if (Bruger.Rolle(HttpContext.Session) != null)
            {
                return RedirectToPage("/Aktiviteter/Index");
            }
            return Page();
        }

        public IActionResult OnPost()
        {
            foreach (Leder l in _lederRepo.HentAlle())
            {
                if (l.BrugerNavn.ToLower() == BrugerNavn.Trim().ToLower() && l.Adgangskode == Adgangskode)
                {
                    Bruger.LogInd(HttpContext.Session, l.Id, l.Navn, Bruger.Leder);
                    TempData["Besked"] = "Velkommen, " + l.Navn;
                    return RedirectToPage("/Aktiviteter/Index");
                }
            }

            foreach (Spejder s in _spejderRepo.HentAlle())
            {
                if (s.BrugerNavn.ToLower() == BrugerNavn.Trim().ToLower() && s.Adgangskode == Adgangskode)
                {
                    Bruger.LogInd(HttpContext.Session, s.Id, s.Navn, Bruger.Spejder);
                    TempData["Besked"] = "Velkommen, " + s.Navn;
                    return RedirectToPage("/Aktiviteter/Index");
                }
            }

            Fejl = "Forkert brugernavn eller adgangskode";
            return Page();
        }
    }
}
