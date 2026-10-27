using DLS_Lib.Repository;
using DLS_Lib.UserStories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DLS_Web.Pages.Aktiviteter
{
    // Lederen ser hvem der er tilmeldt en aktivitet
    public class TilmeldteModel : PageModel
    {
        private AktivitetRepository _aktivitetRepo;
        private TilmeldingRepository _tilmeldingRepo;
        private SpejderRepository _spejderRepo;
        private TegnRepository _tegnRepo;

        public Aktivitet Aktivitet { get; set; } = new Aktivitet();
        public List<Tilmelding> Tilmeldinger { get; set; } = new List<Tilmelding>();

        public TilmeldteModel(AktivitetRepository aktivitetRepo, TilmeldingRepository tilmeldingRepo,
            SpejderRepository spejderRepo, TegnRepository tegnRepo)
        {
            _aktivitetRepo = aktivitetRepo;
            _tilmeldingRepo = tilmeldingRepo;
            _spejderRepo = spejderRepo;
            _tegnRepo = tegnRepo;
        }

        public IActionResult OnGet(int id)
        {
            // Kun ledere må se hvem der er tilmeldt
            if (HttpContext.Session.GetString("Rolle") != "Leder")
            {
                return RedirectToPage("/Login");
            }

            Aktivitet? aktivitet = _aktivitetRepo.HentEn(id);
            if (aktivitet == null)
            {
                return NotFound();
            }

            Aktivitet = aktivitet;
            Tilmeldinger = _tilmeldingRepo.HentForAktivitet(id);
            return Page();
        }

        // Lederen kan fjerne en spejder fra aktiviteten
        public IActionResult OnPostFjern(int id, int spejderId)
        {
            if (HttpContext.Session.GetString("Rolle") != "Leder")
            {
                return RedirectToPage("/Login");
            }

            _tilmeldingRepo.Frameld(spejderId, id);
            TempData["Besked"] = "Spejderen er fjernet fra aktiviteten";
            return RedirectToPage(new { id });
        }

        public Spejder? HentSpejder(int id)
        {
            return _spejderRepo.HentEn(id);
        }

        public string TegnNavne(Spejder spejder)
        {
            string navne = "";
            foreach (int id in spejder.TegnIds)
            {
                Tegn? t = _tegnRepo.HentEn(id);
                if (t != null)
                {
                    if (navne != "")
                    {
                        navne += ", ";
                    }
                    navne += t.Navn;
                }
            }
            return navne;
        }
    }
}
