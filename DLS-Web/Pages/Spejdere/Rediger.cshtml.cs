using DLS_Lib.Repository;
using DLS_Lib.UserStories;
using DLS_Web.Hjaelpere;
using Microsoft.AspNetCore.Mvc;

namespace DLS_Web.Pages.Spejdere
{
    // Bruges både til at oprette (uden id) og rette (med id)
    public class RedigerModel : LederSide
    {
        private SpejderRepository _repo;
        private LederRepository _lederRepo;
        private TegnRepository _tegnRepo;

        [BindProperty]
        public Spejder Spejder { get; set; } = new Spejder();

        public List<Tegn> AlleTegn { get; set; } = new List<Tegn>();

        public bool ErNy
        {
            get { return Spejder.Id == 0; }
        }

        public RedigerModel(SpejderRepository repo, LederRepository lederRepo, TegnRepository tegnRepo)
        {
            _repo = repo;
            _lederRepo = lederRepo;
            _tegnRepo = tegnRepo;
        }

        public IActionResult OnGet(int? id)
        {
            if (id != null)
            {
                Spejder? spejder = _repo.HentEn(id.Value);
                if (spejder == null)
                {
                    return NotFound();
                }
                Spejder = spejder;
            }
            else
            {
                Spejder.Foedselsdato = DateOnly.FromDateTime(DateTime.Today.AddYears(-10));
            }

            AlleTegn = _tegnRepo.HentAlle();
            return Page();
        }

        public IActionResult OnPost()
        {
            Spejder.Navn = Spejder.Navn.Trim();
            Spejder.BrugerNavn = Spejder.BrugerNavn.Trim();

            // Lader man adgangskoden stå tom ved rettelse, beholdes den gamle
            Spejder? gammel = _repo.HentEn(Spejder.Id);
            if (gammel != null && Spejder.Adgangskode == "")
            {
                Spejder.Adgangskode = gammel.Adgangskode;
            }

            if (Spejder.Navn == "")
            {
                ModelState.AddModelError("", "Skriv spejderens navn");
            }
            if (Spejder.BrugerNavn == "")
            {
                ModelState.AddModelError("", "Skriv et brugernavn");
            }
            else if (Bruger.BrugerNavnOptaget(Spejder.BrugerNavn, _repo, _lederRepo, Spejder.Id, 0))
            {
                ModelState.AddModelError("", "Brugernavnet \"" + Spejder.BrugerNavn + "\" er allerede taget");
            }
            if (Spejder.Adgangskode == "")
            {
                ModelState.AddModelError("", "Skriv en adgangskode");
            }
            if (Spejder.Foedselsdato > DateOnly.FromDateTime(DateTime.Today))
            {
                ModelState.AddModelError("", "Fødselsdatoen kan ikke ligge i fremtiden");
            }

            if (!ModelState.IsValid)
            {
                AlleTegn = _tegnRepo.HentAlle();
                return Page();
            }

            if (Spejder.Id == 0)
            {
                _repo.Opret(Spejder);
                TempData["Besked"] = Spejder.Navn + " er oprettet";
            }
            else
            {
                _repo.Opdater(Spejder);
                TempData["Besked"] = Spejder.Navn + " er gemt";
            }
            return RedirectToPage("Index");
        }
    }
}
