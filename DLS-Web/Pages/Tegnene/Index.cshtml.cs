using DLS_Lib.Repository;
using DLS_Lib.UserStories;
using DLS_Web.Hjaelpere;
using Microsoft.AspNetCore.Mvc;

// Mappen hedder "Tegnene", fordi et namespace der hedder "Tegn" ville blive forvekslet med klassen Tegn
namespace DLS_Web.Pages.Tegnene
{
    public class IndexModel : LederSide
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

        public void OnGet()
        {
            Tegn = _repo.HentAlle().OrderBy(t => t.Navn).ToList();
        }

        public IActionResult OnPostSlet(int id)
        {
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

        public int AntalSpejdere(Tegn tegn)
        {
            return _spejderRepo.HentAlle().Count(s => s.TegnIds.Contains(tegn.Id));
        }

        public int AntalAktiviteter(Tegn tegn)
        {
            return _aktivitetRepo.HentAlle().Count(a => a.KraevedeTegnIds.Contains(tegn.Id));
        }
    }
}
