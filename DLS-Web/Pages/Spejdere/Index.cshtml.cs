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

        /// <summary>
        /// Listen af spejdere som vises på siden, sorteret efter navn.
        /// </summary>
        public List<Spejder> Spejdere { get; set; } = new List<Spejder>();

        /// <summary>
        /// Opretter siden med de repositories den skal bruge.
        /// </summary>
        /// <param name="repo">Repository til spejdere.</param>
        /// <param name="tilmeldingRepo">Repository til tilmeldinger.</param>
        /// <param name="tegnRepo">Repository til tegn.</param>
        public IndexModel(SpejderRepository repo, TilmeldingRepository tilmeldingRepo, TegnRepository tegnRepo)
        {
            _repo = repo;
            _tilmeldingRepo = tilmeldingRepo;
            _tegnRepo = tegnRepo;
        }

        /// <summary>
        /// Viser listen af spejdere. Kun ledere har adgang.
        /// </summary>
        /// <returns>Siden, eller en omdirigering til login hvis brugeren ikke er leder.</returns>
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

        /// <summary>
        /// Sletter en spejder og spejderens tilmeldinger. Kun ledere må slette.
        /// </summary>
        /// <param name="id">Id på den spejder der skal slettes.</param>
        /// <returns>En omdirigering tilbage til siden, eller til login hvis brugeren ikke er leder.</returns>
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

        /// <summary>
        /// Tæller hvor mange aktiviteter en spejder er tilmeldt.
        /// </summary>
        /// <param name="spejder">Spejderen der skal tælles for.</param>
        /// <returns>Antallet af spejderens tilmeldinger.</returns>
        public int AntalTilmeldinger(Spejder spejder)
        {
            return _tilmeldingRepo.HentForSpejder(spejder.Id).Count;
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
