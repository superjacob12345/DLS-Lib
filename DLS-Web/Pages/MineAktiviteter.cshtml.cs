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

        public List<Aktivitet> Aktiviteter { get; set; } = new List<Aktivitet>();
        public List<Tegn> MineTegn { get; set; } = new List<Tegn>();
        public Spejder MigSelv { get; set; } = new Spejder();

        public MineAktiviteterModel(AktivitetRepository aktivitetRepo, TilmeldingRepository tilmeldingRepo,
            SpejderRepository spejderRepo, TegnRepository tegnRepo)
        {
            _aktivitetRepo = aktivitetRepo;
            _tilmeldingRepo = tilmeldingRepo;
            _spejderRepo = spejderRepo;
            _tegnRepo = tegnRepo;
        }

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
