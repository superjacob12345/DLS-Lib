using DLS_Lib.Repository;
using DLS_Lib.UserStories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DLS_Web.Pages.Spejdere
{
    // Bruges både til at oprette (uden id) og rette (med id)
    public class RedigerModel : PageModel
    {
        private SpejderRepository _repo;
        private LederRepository _lederRepo;
        private TegnRepository _tegnRepo;

        /// <summary>
        /// Den spejder der oprettes eller rettes i formularen.
        /// </summary>
        [BindProperty]
        public Spejder Spejder { get; set; } = new Spejder();

        /// <summary>
        /// Alle tegn, som spejderen kan have, og som vises i formularen.
        /// </summary>
        public List<Tegn> AlleTegn { get; set; } = new List<Tegn>();

        /// <summary>
        /// Angiver om spejderen er ny og endnu ikke gemt (Id er 0).
        /// </summary>
        public bool ErNy
        {
            get { return Spejder.Id == 0; }
        }

        /// <summary>
        /// Opretter siden med de repositories den skal bruge.
        /// </summary>
        /// <param name="repo">Repository til spejdere.</param>
        /// <param name="lederRepo">Repository til ledere.</param>
        /// <param name="tegnRepo">Repository til tegn.</param>
        public RedigerModel(SpejderRepository repo, LederRepository lederRepo, TegnRepository tegnRepo)
        {
            _repo = repo;
            _lederRepo = lederRepo;
            _tegnRepo = tegnRepo;
        }

        /// <summary>
        /// Viser formularen. Uden id får fødselsdatoen en startværdi på 10 år tilbage, og med id hentes den spejder der skal rettes. Kun ledere har adgang.
        /// </summary>
        /// <param name="id">Id på den spejder der skal rettes, eller null ved oprettelse.</param>
        /// <returns>Siden, en omdirigering til login hvis brugeren ikke er leder, eller NotFound hvis spejderen ikke findes.</returns>
        public IActionResult OnGet(int? id)
        {
            // Kun ledere må oprette og rette spejdere
            if (HttpContext.Session.GetString("Rolle") != "Leder")
            {
                return RedirectToPage("/Login");
            }

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
                Spejder.Foeselsdag = DateOnly.FromDateTime(DateTime.Today.AddYears(-10));
            }

            AlleTegn = _tegnRepo.HentAlle();
            return Page();
        }

        /// <summary>
        /// Validerer formularen og opretter eller opdaterer spejderen. Lader man adgangskoden stå tom ved rettelse, beholdes den gamle. Kun ledere har adgang.
        /// </summary>
        /// <returns>En omdirigering til oversigten når spejderen er gemt, siden igen ved valideringsfejl, en omdirigering til login hvis brugeren ikke er leder, eller NotFound hvis spejderen ikke findes.</returns>
        public IActionResult OnPost()
        {
            if (HttpContext.Session.GetString("Rolle") != "Leder")
            {
                return RedirectToPage("/Login");
            }

            // Tomme tekstfelter kommer ind som null, så de laves om til ""
            if (Spejder.Navn == null)
            {
                Spejder.Navn = "";
            }
            if (Spejder.BrugerNavn == null)
            {
                Spejder.BrugerNavn = "";
            }
            if (Spejder.Adgangskode == null)
            {
                Spejder.Adgangskode = "";
            }
            Spejder.Navn = Spejder.Navn.Trim();
            Spejder.BrugerNavn = Spejder.BrugerNavn.Trim();

            // Reglerne på Spejder-klassen ([Required], [StringLength], [Foedselsdato] osv.)
            // er allerede tjekket af ASP.NET og ligger i ModelState.
            // Her tilføjes de regler der kræver mere end selve feltet:

            // En ny spejder skal have en adgangskode
            if (ErNy && Spejder.Adgangskode == "")
            {
                ModelState.AddModelError("Spejder.Adgangskode", "Skriv en adgangskode");
            }

            // Brugernavnet må ikke være taget
            if (Spejder.BrugerNavn != "" && BrugerNavnOptaget())
            {
                ModelState.AddModelError("Spejder.BrugerNavn", "Brugernavnet \"" + Spejder.BrugerNavn + "\" er allerede taget");
            }

            // Den spejder man retter skal findes
            Spejder? gammel = _repo.HentEn(Spejder.Id);
            if (!ErNy && gammel == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                AlleTegn = _tegnRepo.HentAlle();
                return Page();
            }

            // Lader man adgangskoden stå tom ved rettelse, beholdes den gamle
            if (gammel != null && Spejder.Adgangskode == "")
            {
                Spejder.Adgangskode = gammel.Adgangskode;
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

        // Brugernavnet må ikke bruges af en anden spejder eller en leder
        private bool BrugerNavnOptaget()
        {
            string navn = Spejder.BrugerNavn.ToLower();
            foreach (Spejder s in _repo.HentAlle())
            {
                if (s.Id != Spejder.Id && s.BrugerNavn.ToLower() == navn)
                {
                    return true;
                }
            }
            foreach (Leder l in _lederRepo.HentAlle())
            {
                if (l.BrugerNavn.ToLower() == navn)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
