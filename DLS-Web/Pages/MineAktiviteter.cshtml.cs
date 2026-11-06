using DLS_Lib.Repository;
using DLS_Lib.UserStories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DLS_Web.Pages
{
    // Spejderen ser de aktiviteter spejderen er tilmeldt
    public class MineAktiviteterModel : PageModel
    {
        private AktivitetRepository _aktivitetRepo;
        private TilmeldingRepository _tilmeldingRepo;
        private SpejderRepository _spejderRepo;
        private TegnRepository _tegnRepo;

        /// <summary>
        /// De aktiviteter spejderen er tilmeldt, sorteret efter tidspunkt.
        /// </summary>
        public List<Aktivitet> Aktiviteter { get; set; } = new List<Aktivitet>();
        /// <summary>
        /// De tegn spejderen har taget.
        /// </summary>
        public List<Tegn> MineTegn { get; set; } = new List<Tegn>();
        /// <summary>
        /// Den spejder der er logget ind.
        /// </summary>
        public Spejder MigSelv { get; set; } = new Spejder();

        /// <summary>
        /// Opretter siden med de repositories den skal bruge.
        /// </summary>
        /// <param name="aktivitetRepo">Repository til aktiviteter.</param>
        /// <param name="tilmeldingRepo">Repository til tilmeldinger.</param>
        /// <param name="spejderRepo">Repository til spejdere.</param>
        /// <param name="tegnRepo">Repository til tegn.</param>
        public MineAktiviteterModel(AktivitetRepository aktivitetRepo, TilmeldingRepository tilmeldingRepo,
            SpejderRepository spejderRepo, TegnRepository tegnRepo)
        {
            _aktivitetRepo = aktivitetRepo;
            _tilmeldingRepo = tilmeldingRepo;
            _spejderRepo = spejderRepo;
            _tegnRepo = tegnRepo;
        }

        /// <summary>
        /// Viser den indloggede spejders aktiviteter og tegn. Kun spejdere har adgang.
        /// </summary>
        /// <returns>Siden, eller en omdirigering til login hvis brugeren ikke er spejder eller spejderen er blevet slettet.</returns>
        public IActionResult OnGet()
        {
            // Kun spejdere har en liste over egne aktiviteter
            if (HttpContext.Session.GetString("Rolle") != "Spejder")
            {
                return RedirectToPage("/Login");
            }

            Spejder? spejder = _spejderRepo.HentEn(HttpContext.Session.GetInt32("BrugerId") ?? 0);
            if (spejder == null)
            {
                // Spejderen er blevet slettet, mens spejderen var logget ind
                HttpContext.Session.Clear();
                return RedirectToPage("/Login");
            }
            MigSelv = spejder;

            foreach (Tilmelding t in _tilmeldingRepo.HentForSpejder(spejder.Id))
            {
                Aktivitet? a = _aktivitetRepo.HentEn(t.AktivitetId);
                if (a != null)
                {
                    Aktiviteter.Add(a);
                }
            }
            Aktiviteter = Aktiviteter.OrderBy(a => a.Tidspunkt).ToList();

            foreach (int id in spejder.TegnIds)
            {
                Tegn? tegn = _tegnRepo.HentEn(id);
                if (tegn != null)
                {
                    MineTegn.Add(tegn);
                }
            }
            return Page();
        }

        /// <summary>
        /// Framelder den spejder der er logget ind, fra en aktivitet.
        /// </summary>
        /// <param name="id">Id på aktiviteten.</param>
        /// <returns>En omdirigering tilbage til siden, eller til login hvis brugeren ikke er spejder.</returns>
        public IActionResult OnPostFrameld(int id)
        {
            if (HttpContext.Session.GetString("Rolle") != "Spejder")
            {
                return RedirectToPage("/Login");
            }

            _tilmeldingRepo.Frameld(HttpContext.Session.GetInt32("BrugerId") ?? 0, id);
            TempData["Besked"] = "Du er frameldt aktiviteten";
            return RedirectToPage();
        }
    }
}
