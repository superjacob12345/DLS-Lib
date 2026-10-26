using DLS_Lib.Repository;
using DLS_Lib.UserStories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DLS_Web.Pages.Ledere
{
    public class IndexModel : PageModel
    {
        private LederRepository _repo;
        private AktivitetRepository _aktivitetRepo;

        // Listen som vises på siden
        public List<Leder> Ledere { get; set; } = new List<Leder>();

        // Den leder der er logget ind (kan ikke slette sig selv)
        public int MitId { get; set; }

        public IndexModel(LederRepository repo, AktivitetRepository aktivitetRepo)
        {
            _repo = repo;
            _aktivitetRepo = aktivitetRepo;
        }

        public IActionResult OnGet()
        {
            // Kun ledere må se siden
            if (HttpContext.Session.GetString("Rolle") != "Leder")
            {
                return RedirectToPage("/Login");
            }

            Ledere = _repo.HentAlle().OrderBy(l => l.Navn).ToList();
            MitId = HttpContext.Session.GetInt32("BrugerId") ?? 0;
            return Page();
        }

        public IActionResult OnPostSlet(int id)
        {
            if (HttpContext.Session.GetString("Rolle") != "Leder")
            {
                return RedirectToPage("/Login");
            }

            Leder? leder = _repo.HentEn(id);
            if (leder == null)
            {
                return RedirectToPage();
            }

            if (id == HttpContext.Session.GetInt32("BrugerId"))
            {
                TempData["Fejl"] = "Du kan ikke slette dig selv";
                return RedirectToPage();
            }

            // En aktivitet skal have mindst én leder
            foreach (Aktivitet a in _aktivitetRepo.HentAlle())
            {
                if (a.LederIds.Contains(id) && a.LederIds.Count == 1)
                {
                    TempData["Fejl"] = leder.Navn + " er eneste leder på \"" + a.Navn + "\". Giv aktiviteten en anden leder først.";
                    return RedirectToPage();
                }
            }

            foreach (Aktivitet a in _aktivitetRepo.HentAlle())
            {
                a.LederIds.Remove(id);
            }
            _repo.Slet(id);
            TempData["Besked"] = leder.Navn + " er slettet";
            return RedirectToPage();
        }

        // De aktiviteter lederen står for
        public List<Aktivitet> HentAktiviteter(Leder leder)
        {
            List<Aktivitet> resultat = new List<Aktivitet>();
            foreach (Aktivitet a in _aktivitetRepo.HentAlle())
            {
                if (a.LederIds.Contains(leder.Id))
                {
                    resultat.Add(a);
                }
            }
            return resultat;
        }
    }
}
