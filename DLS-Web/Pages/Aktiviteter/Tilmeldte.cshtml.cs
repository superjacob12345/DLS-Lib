using DLS_Lib.Repository;
using DLS_Lib.UserStories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DLS_Web.Pages.Aktiviteter
{
    // Lederen ser hvem der er tilmeldt en aktivitet
    public class TilmeldteModel : PageModel
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
            Spejder? spejder = _spejderRepo.HentEn(spejderId);
            _tilmeldingRepo.Frameld(spejderId, id);
            if (spejder != null)
            {
                TempData["Besked"] = spejder.Navn + " er fjernet fra aktiviteten";
            }
            return RedirectToPage(new { id });
        }

        public Spejder? HentSpejder(int id)
        {
            return _spejderRepo.HentEn(id);
        }

        public string TegnNavne(Spejder spejder)
        {
            List<string> navne = new List<string>();
            foreach (int id in spejder.TegnIds)
            {
                Tegn? t = _tegnRepo.HentEn(id);
                if (t != null)
                {
                    navne.Add(t.Navn);
                }
            }
            return string.Join(", ", navne);
        }
    }
}
