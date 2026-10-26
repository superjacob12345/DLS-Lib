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

        // Listen som vises på siden
        public List<Tegn> Tegn { get; set; } = new List<Tegn>();

        public IndexModel(TegnRepository repo, SpejderRepository spejderRepo, AktivitetRepository aktivitetRepo)
        {
            _repo = repo;
            _spejderRepo = spejderRepo;
            _aktivitetRepo = aktivitetRepo;
        }

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

        public IActionResult OnPostSlet(int id)
        {
            if (HttpContext.Session.GetString("Rolle") != "Leder")
            {
                return RedirectToPage("/Login");
            }

            Tegn? tegn = _repo.HentEn(id);
            if (tegn != null)
            {
                // Fjern tegnet fra spejdere og aktiviteter, så ingen står med et tegn der ikke findes
                foreach (Spejder s in _spejderRepo.HentAlle())
                {
                    s.TegnIds.Remove(id);
                }
                foreach (Aktivitet a in _aktivitetRepo.HentAlle())
                {
                    a.KraevedeTegnIds.Remove(id);
                }
                _repo.Slet(id);
                TempData["Besked"] = tegn.Navn + " er slettet";
            }
            return RedirectToPage();
        }

        // Hvor mange spejdere har tegnet
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

        // Hvor mange aktiviteter kræver tegnet
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
