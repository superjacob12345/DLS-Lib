using DLS_Lib.Repository;
using DLS_Lib.UserStories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

// Mappen hedder "Tegnene", fordi et namespace der hedder "Tegn" ville blive forvekslet med klassen Tegn
namespace DLS_Web.Pages.Tegnene
{
    public class IndexModel : PageModel
    {
        private TegnRepository _repo;
        private SpejderRepository _spejderRepo;
        private AktivitetRepository _aktivitetRepo;

        /// <summary>
        /// Listen af tegn som vises på siden, sorteret efter navn.
        /// </summary>
        public List<Tegn> Tegn { get; set; } = new List<Tegn>();

        /// <summary>
        /// Opretter siden med de repositories den skal bruge.
        /// </summary>
        /// <param name="repo">Repository til tegn.</param>
        /// <param name="spejderRepo">Repository til spejdere.</param>
        /// <param name="aktivitetRepo">Repository til aktiviteter.</param>
        public IndexModel(TegnRepository repo, SpejderRepository spejderRepo, AktivitetRepository aktivitetRepo)
        {
            _repo = repo;
            _spejderRepo = spejderRepo;
            _aktivitetRepo = aktivitetRepo;
        }

        /// <summary>
        /// Viser listen af tegn. Kun ledere har adgang.
        /// </summary>
        /// <returns>Siden, eller en omdirigering til login hvis brugeren ikke er leder.</returns>
        public IActionResult OnGet()
        {
            // Kun ledere må se siden
            if (HttpContext.Session.GetString("Rolle") != "Leder")
            {
                return RedirectToPage("/Login");
            }

            Tegn = _repo.HentAlle().OrderBy(t => t.Navn).ToList();
            return Page();
        }

        /// <summary>
        /// Sletter et tegn. Kun ledere må slette.
        /// </summary>
        /// <param name="id">Id på det tegn der skal slettes.</param>
        /// <returns>En omdirigering tilbage til siden, eller til login hvis brugeren ikke er leder.</returns>
        public IActionResult OnPostSlet(int id)
        {
            if (HttpContext.Session.GetString("Rolle") != "Leder")
            {
                return RedirectToPage("/Login");
            }

            // Tegnet bliver også fjernet fra spejdere og aktiviteter, fordi tabellerne har ON DELETE CASCADE
            _repo.Slet(id);
            TempData["Besked"] = "Tegnet er slettet";
            return RedirectToPage();
        }

        /// <summary>
        /// Tæller hvor mange spejdere der har et tegn.
        /// </summary>
        /// <param name="tegn">Tegnet der skal tælles for.</param>
        /// <returns>Antallet af spejdere der har tegnet.</returns>
        public int AntalSpejdere(Tegn tegn)
        {
            int antal = 0;
            foreach (Spejder s in _spejderRepo.HentAlle())
            {
                if (s.TegnIds.Contains(tegn.Id))
                {
                    antal++;
                }
            }
            return antal;
        }

        /// <summary>
        /// Tæller hvor mange aktiviteter der kræver et tegn.
        /// </summary>
        /// <param name="tegn">Tegnet der skal tælles for.</param>
        /// <returns>Antallet af aktiviteter der kræver tegnet.</returns>
        public int AntalAktiviteter(Tegn tegn)
        {
            int antal = 0;
            foreach (Aktivitet a in _aktivitetRepo.HentAlle())
            {
                if (a.KraevedeTegnIds.Contains(tegn.Id))
                {
                    antal++;
                }
            }
            return antal;
        }
    }
}
