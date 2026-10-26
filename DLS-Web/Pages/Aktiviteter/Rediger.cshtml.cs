using DLS_Lib.Repository;
using DLS_Lib.UserStories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DLS_Web.Pages.Aktiviteter
{
    // Bruges både til at oprette (uden id) og rette (med id)
    public class RedigerModel : PageModel
    {
        private AktivitetRepository _aktivitetRepo;
        private LederRepository _lederRepo;
        private TegnRepository _tegnRepo;
        private TilmeldingRepository _tilmeldingRepo;

        [BindProperty]
        public Aktivitet Aktivitet { get; set; } = new Aktivitet();

        // Til afkrydsningsfelterne
        public List<Leder> AlleLedere { get; set; } = new List<Leder>();
        public List<Tegn> AlleTegn { get; set; } = new List<Tegn>();

        // Fejlbeskeder der vises over formularen
        public List<string> Fejl { get; set; } = new List<string>();

        public bool ErNy
        {
            get { return Aktivitet.Id == 0; }
        }

        public RedigerModel(AktivitetRepository aktivitetRepo, LederRepository lederRepo,
            TegnRepository tegnRepo, TilmeldingRepository tilmeldingRepo)
        {
            _aktivitetRepo = aktivitetRepo;
            _lederRepo = lederRepo;
            _tegnRepo = tegnRepo;
            _tilmeldingRepo = tilmeldingRepo;
        }

        public IActionResult OnGet(int? id)
        {
            // Kun ledere må oprette og rette aktiviteter
            if (HttpContext.Session.GetString("Rolle") != "Leder")
            {
                return RedirectToPage("/Login");
            }

            if (id == null)
            {
                // Ny aktivitet: forslag til tidspunkt, og lederen der opretter står for den
                Aktivitet.Tidspunkt = DateTime.Today.AddDays(1).AddHours(10);
                Aktivitet.MaksDeltagere = 10;
                Aktivitet.LederIds.Add(HttpContext.Session.GetInt32("BrugerId") ?? 0);
            }
            else
            {
                Aktivitet? aktivitet = _aktivitetRepo.HentEn(id.Value);
                if (aktivitet == null)
                {
                    return NotFound();
                }
                Aktivitet = aktivitet;
            }

            AlleLedere = _lederRepo.HentAlle();
            AlleTegn = _tegnRepo.HentAlle();
            return Page();
        }

        public IActionResult OnPost()
        {
            if (HttpContext.Session.GetString("Rolle") != "Leder")
            {
                return RedirectToPage("/Login");
            }

            // Tomme tekstfelter kommer ind som null, så de laves om til ""
            if (Aktivitet.Navn == null)
            {
                Aktivitet.Navn = "";
            }
            if (Aktivitet.Sted == null)
            {
                Aktivitet.Sted = "";
            }
            if (Aktivitet.Beskrivelse == null)
            {
                Aktivitet.Beskrivelse = "";
            }

            // Tjek at felterne er udfyldt rigtigt
            if (Aktivitet.Navn.Trim() == "")
            {
                Fejl.Add("Aktiviteten skal have et navn");
            }
            if (Aktivitet.MaksDeltagere < 1)
            {
                Fejl.Add("Der skal være plads til mindst 1 spejder");
            }
            if (Aktivitet.MinAlder < 0)
            {
                Fejl.Add("Minimumsalderen kan ikke være negativ");
            }
            if (Aktivitet.LederIds.Count == 0)
            {
                Fejl.Add("Vælg mindst én leder der står for aktiviteten");
            }
            if (Aktivitet.Id != 0)
            {
                int tilmeldte = _tilmeldingRepo.HentForAktivitet(Aktivitet.Id).Count;
                if (Aktivitet.MaksDeltagere < tilmeldte)
                {
                    Fejl.Add("Der er allerede " + tilmeldte + " tilmeldt, så maks kan ikke være lavere");
                }
            }

            if (Fejl.Count > 0)
            {
                AlleLedere = _lederRepo.HentAlle();
                AlleTegn = _tegnRepo.HentAlle();
                return Page();
            }

            if (Aktivitet.Id == 0)
            {
                _aktivitetRepo.Opret(Aktivitet);
                TempData["Besked"] = Aktivitet.Navn + " er oprettet";
            }
            else
            {
                _aktivitetRepo.Opdater(Aktivitet);
                TempData["Besked"] = Aktivitet.Navn + " er gemt";
            }
            return RedirectToPage("Index");
        }
    }
}
