using DLS_Lib.Repository;
using DLS_Lib.UserStories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DLS_Web.Pages.Tilmeldinger
{
    public class IndexModel : PageModel
    {
        private TilmeldingRepository _tilmeldingRepo;
        private SpejderRepository _spejderRepo;
        private AktivitetRepository _aktivitetRepo;

        public List<Tilmelding> Tilmeldinger { get; set; } = new List<Tilmelding>();
        public List<Spejder> Spejdere { get; set; } = new List<Spejder>();
        public List<Aktivitet> Aktiviteter { get; set; } = new List<Aktivitet>();

        // Det der vælges i formularen
        [BindProperty]
        public int SpejderId { get; set; }

        [BindProperty]
        public int AktivitetId { get; set; }

        // Beskeden der vises efter en tilmelding
        public string Fejl { get; set; } = "";

        public IndexModel(TilmeldingRepository tilmeldingRepo, SpejderRepository spejderRepo, AktivitetRepository aktivitetRepo)
        {
            _tilmeldingRepo = tilmeldingRepo;
            _spejderRepo = spejderRepo;
            _aktivitetRepo = aktivitetRepo;
        }

        public void OnGet()
        {
            HentLister();
        }

        public IActionResult OnPostTilmeld()
        {
            Spejder? spejder = _spejderRepo.HentEn(SpejderId);
            Aktivitet? aktivitet = _aktivitetRepo.HentEn(AktivitetId);

            if (spejder == null || aktivitet == null)
            {
                Fejl = "Vælg både en spejder og en aktivitet";
                HentLister();
                return Page();
            }

            // Tilmeld returnerer "" hvis det gik godt, ellers en fejlbesked
            Fejl = _tilmeldingRepo.Tilmeld(spejder, aktivitet);
            if (Fejl != "")
            {
                HentLister();
                return Page();
            }

            return RedirectToPage();
        }

        public IActionResult OnPostFrameld(int spejderId, int aktivitetId)
        {
            _tilmeldingRepo.Frameld(spejderId, aktivitetId);
            return RedirectToPage();
        }

        // Bruges på siden til at vise navne i stedet for Id'er
        public string SpejderNavn(int id)
        {
            Spejder? s = _spejderRepo.HentEn(id);
            if (s == null)
            {
                return "(slettet)";
            }
            return s.Navn;
        }

        public string AktivitetNavn(int id)
        {
            Aktivitet? a = _aktivitetRepo.HentEn(id);
            if (a == null)
            {
                return "(slettet)";
            }
            return a.Navn;
        }

        private void HentLister()
        {
            Tilmeldinger = _tilmeldingRepo.HentAlle();
            Spejdere = _spejderRepo.HentAlle();
            Aktiviteter = _aktivitetRepo.HentAlle();
        }
    }
}
