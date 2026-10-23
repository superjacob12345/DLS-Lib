using DLS_Lib.Repository;
using DLS_Lib.UserStories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DLS_Web.Pages.Ledere
{
    public class IndexModel : PageModel
    {
        // Kun ledere har adgang til siden. Kører før OnGet/OnPost.
        public override void OnPageHandlerExecuting(PageHandlerExecutingContext context)
        {
            string? rolle = HttpContext.Session.GetString("Rolle");
            if (rolle == null)
            {
                TempData["Fejl"] = "Du skal logge ind først";
                context.Result = RedirectToPage("/Login");
            }
            else if (rolle != "Leder")
            {
                TempData["Fejl"] = "Den side er kun for ledere";
                context.Result = RedirectToPage("/Index");
            }
        }

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

        public void OnGet()
        {
            Ledere = _repo.HentAlle().OrderBy(l => l.Navn).ToList();
            MitId = (HttpContext.Session.GetInt32("BrugerId") ?? 0);
        }

        public IActionResult OnPostSlet(int id)
        {
            Leder? leder = _repo.HentEn(id);
            if (leder == null)
            {
                return RedirectToPage();
            }

            if (id == (HttpContext.Session.GetInt32("BrugerId") ?? 0))
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

        public List<Aktivitet> HentAktiviteter(Leder leder)
        {
            return _aktivitetRepo.HentAlle()
                .Where(a => a.LederIds.Contains(leder.Id))
                .OrderBy(a => a.Tidspunkt)
                .ToList();
        }
    }
}
