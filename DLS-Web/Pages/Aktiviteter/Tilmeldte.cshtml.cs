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

        /// <summary>
        /// Den aktivitet der vises tilmeldinger for.
        /// </summary>
        public Aktivitet Aktivitet { get; set; } = new Aktivitet();
        /// <summary>
        /// Tilmeldingerne til aktiviteten.
        /// </summary>
        public List<Tilmelding> Tilmeldinger { get; set; } = new List<Tilmelding>();

        /// <summary>
        /// Opretter siden med de repositories den skal bruge.
        /// </summary>
        /// <param name="aktivitetRepo">Repository til aktiviteter.</param>
        /// <param name="tilmeldingRepo">Repository til tilmeldinger.</param>
        /// <param name="spejderRepo">Repository til spejdere.</param>
        /// <param name="tegnRepo">Repository til tegn.</param>
        public TilmeldteModel(AktivitetRepository aktivitetRepo, TilmeldingRepository tilmeldingRepo,
            SpejderRepository spejderRepo, TegnRepository tegnRepo)
        {
            _aktivitetRepo = aktivitetRepo;
            _tilmeldingRepo = tilmeldingRepo;
            _spejderRepo = spejderRepo;
            _tegnRepo = tegnRepo;
        }

        /// <summary>
        /// Viser hvem der er tilmeldt en aktivitet. Kun ledere har adgang.
        /// </summary>
        /// <param name="id">Id på aktiviteten.</param>
        /// <returns>Siden, en omdirigering til login hvis brugeren ikke er leder, eller NotFound hvis aktiviteten ikke findes.</returns>
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

        /// <summary>
        /// Lader en leder fjerne en spejder fra aktiviteten.
        /// </summary>
        /// <param name="id">Id på aktiviteten.</param>
        /// <param name="spejderId">Id på den spejder der skal fjernes.</param>
        /// <returns>En omdirigering tilbage til siden, eller til login hvis brugeren ikke er leder.</returns>
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

        /// <summary>
        /// Henter en spejder ud fra spejderens id.
        /// </summary>
        /// <param name="id">Id på spejderen.</param>
        /// <returns>Spejderen, eller null hvis spejderen ikke findes.</returns>
        public Spejder? HentSpejder(int id)
        {
            return _spejderRepo.HentEn(id);
        }

        /// <summary>
        /// Laver en kommasepareret tekst med navnene på en spejders tegn.
        /// </summary>
        /// <param name="spejder">Spejderen hvis tegn skal findes.</param>
        /// <returns>Tegnenes navne adskilt af komma. Teksten er tom hvis spejderen ingen tegn har.</returns>
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
