using DLS_Lib.Repository;
using DLS_Lib.UserStories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DLS_Web.Pages.Spejdere
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

        private SpejderRepository _repo;
        private TilmeldingRepository _tilmeldingRepo;
        private TegnRepository _tegnRepo;

        // Listen som vises på siden
        public List<Spejder> Spejdere { get; set; } = new List<Spejder>();

        public IndexModel(SpejderRepository repo, TilmeldingRepository tilmeldingRepo, TegnRepository tegnRepo)
        {
            _repo = repo;
            _tilmeldingRepo = tilmeldingRepo;
            _tegnRepo = tegnRepo;
        }

        public void OnGet()
        {
            Spejdere = _repo.HentAlle().OrderBy(s => s.Navn).ToList();
        }

        public IActionResult OnPostSlet(int id)
        {
            Spejder? spejder = _repo.HentEn(id);
            if (spejder != null)
            {
                _tilmeldingRepo.SletForSpejder(id);
                _repo.Slet(id);
                TempData["Besked"] = spejder.Navn + " er slettet";
            }
            return RedirectToPage();
        }

        public int AntalTilmeldinger(Spejder spejder)
        {
            return _tilmeldingRepo.HentForSpejder(spejder.Id).Count;
        }

        public List<Tegn> HentTegn(Spejder spejder)
        {
            List<Tegn> tegn = new List<Tegn>();
            foreach (int id in spejder.TegnIds)
            {
                Tegn? t = _tegnRepo.HentEn(id);
                if (t != null)
                {
                    tegn.Add(t);
                }
            }
            return tegn;
        }
    }
}
