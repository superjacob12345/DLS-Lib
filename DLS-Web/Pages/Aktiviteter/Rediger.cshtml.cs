using DLS_Lib.Repository;
using DLS_Lib.UserStories;
using DLS_Web.Hjaelpere;
using Microsoft.AspNetCore.Mvc;

namespace DLS_Web.Pages.Aktiviteter
{
    // Bruges både til at oprette (uden id) og rette (med id)
    public class RedigerModel : LederSide
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
            if (id == null)
            {
                // Ny aktivitet: forslag til tidspunkt, og lederen der opretter står for den
                Aktivitet.Tidspunkt = DateTime.Today.AddDays(1).AddHours(10);
                Aktivitet.MaksDeltagere = 10;
                Aktivitet.LederIds = new List<int> { Bruger.Id(HttpContext.Session) };
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

            HentValg();
            return Page();
        }

        public IActionResult OnPost()
        {
            Aktivitet.Navn = Aktivitet.Navn.Trim();

            if (Aktivitet.Navn == "")
            {
                ModelState.AddModelError("", "Aktiviteten skal have et navn");
            }
            if (Aktivitet.MaksDeltagere < 1)
            {
                ModelState.AddModelError("", "Der skal være plads til mindst 1 spejder");
            }
            if (Aktivitet.MinAlder < 0)
            {
                ModelState.AddModelError("", "Minimumsalderen kan ikke være negativ");
            }
            if (Aktivitet.LederIds.Count == 0)
            {
                ModelState.AddModelError("", "Vælg mindst én leder der står for aktiviteten");
            }
            if (Aktivitet.Id != 0)
            {
                int tilmeldte = _tilmeldingRepo.HentForAktivitet(Aktivitet.Id).Count;
                if (Aktivitet.MaksDeltagere < tilmeldte)
                {
                    ModelState.AddModelError("", "Der er allerede " + tilmeldte + " tilmeldt, så maks kan ikke være lavere");
                }
            }

            if (!ModelState.IsValid)
            {
                HentValg();
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

        private void HentValg()
        {
            AlleLedere = _lederRepo.HentAlle();
            AlleTegn = _tegnRepo.HentAlle();
        }
    }
}
