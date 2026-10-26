using DLS_Lib.Repository;
using DLS_Lib.UserStories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DLS_Web.Pages.Spejdere
{
    public class IndexModel : PageModel
    {
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

        public IActionResult OnGet()
        {
            // Kun ledere må se siden
            if (HttpContext.Session.GetString("Rolle") != "Leder")
            {
                return RedirectToPage("/Login");
            }

            Spejdere = _repo.HentAlle().OrderBy(s => s.Navn).ToList();
            return Page();
        }

        public IActionResult OnPostSlet(int id)
        {
            if (HttpContext.Session.GetString("Rolle") != "Leder")
            {
                return RedirectToPage("/Login");
            }

            Spejder? spejder = _repo.HentEn(id);
            if (spejder != null)
            {
                // Spejderens tilmeldinger skal også væk
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
