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

        /// <summary>
        /// Det brugernavn der er skrevet i formularen.
        /// </summary>
        [BindProperty]
        public string BrugerNavn { get; set; } = "";

        /// <summary>
        /// Den adgangskode der er skrevet i formularen.
        /// </summary>
        [BindProperty]
        public string Adgangskode { get; set; } = "";

        /// <summary>
        /// Fejlbesked der vises hvis login mislykkes. Er tom hvis der ingen fejl er.
        /// </summary>
        public string Fejl { get; set; } = "";

        /// <summary>
        /// Opretter siden med de repositories den skal bruge.
        /// </summary>
        /// <param name="spejderRepo">Repository til spejdere.</param>
        /// <param name="lederRepo">Repository til ledere.</param>
        public LoginModel(SpejderRepository spejderRepo, LederRepository lederRepo)
        {
            _spejderRepo = spejderRepo;
            _lederRepo = lederRepo;
        }

        /// <summary>
        /// Viser login-siden, medmindre man allerede er logget ind.
        /// </summary>
        /// <returns>Siden, eller en omdirigering til aktiviteterne hvis man allerede er logget ind.</returns>
        public IActionResult OnGet()
        {
            // Er man allerede logget ind, er der ingen grund til at vise login
            if (HttpContext.Session.GetString("Rolle") != null)
            {
                return RedirectToPage("/Aktiviteter/Index");
            }
            return Page();
        }

        /// <summary>
        /// Logger brugeren ind. Først tjekkes om oplysningerne passer på en leder, og ellers om de passer på en spejder. Brugerens id, navn og rolle gemmes i sessionen.
        /// </summary>
        /// <returns>En omdirigering til aktiviteterne når login lykkes, ellers siden igen med en fejlbesked.</returns>
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
