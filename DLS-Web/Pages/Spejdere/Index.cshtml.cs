using DLS_Lib.Repository;
using DLS_Lib.UserStories;
using DLS_Web.Hjaelpere;
using Microsoft.AspNetCore.Mvc;

namespace DLS_Web.Pages.Spejdere
{
    public class IndexModel : LederSide
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
