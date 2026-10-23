using DLS_Lib.Repository;
using DLS_Lib.UserStories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DLS_Web.Pages
{
    // Spejderen ser de aktiviteter spejderen er tilmeldt
    public class MineAktiviteterModel : PageModel
    {
        // Kun spejdere har adgang til siden. Kører før OnGet/OnPost.
        public override void OnPageHandlerExecuting(PageHandlerExecutingContext context)
        {
            string? rolle = HttpContext.Session.GetString("Rolle");
            if (rolle == null)
            {
                TempData["Fejl"] = "Du skal logge ind først";
                context.Result = RedirectToPage("/Login");
            }
            else if (rolle != "Spejder")
            {
                TempData["Fejl"] = "Den side er kun for spejdere";
                context.Result = RedirectToPage("/Index");
            }
        }

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
            Spejder? spejder = _spejderRepo.HentEn((HttpContext.Session.GetInt32("BrugerId") ?? 0));
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
            Aktivitet? aktivitet = _aktivitetRepo.HentEn(id);
            _tilmeldingRepo.Frameld((HttpContext.Session.GetInt32("BrugerId") ?? 0), id);
            if (aktivitet != null)
            {
                TempData["Besked"] = "Du er frameldt " + aktivitet.Navn;
            }
            return RedirectToPage();
        }
    }
}
