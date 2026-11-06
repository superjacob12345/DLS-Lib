using DLS_Lib.Repository;
using DLS_Lib.UserStories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
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

        /// <summary>
        /// Den aktivitet der oprettes eller rettes i formularen.
        /// </summary>
        [BindProperty]
        public Aktivitet Aktivitet { get; set; } = new Aktivitet();

        /// <summary>
        /// Alle ledere, som vises som afkrydsningsfelter i formularen.
        /// </summary>
        public List<Leder> AlleLedere { get; set; } = new List<Leder>();
        /// <summary>
        /// Alle tegn, som vises som afkrydsningsfelter i formularen.
        /// </summary>
        public List<Tegn> AlleTegn { get; set; } = new List<Tegn>();

        /// <summary>
        /// Angiver om aktiviteten er ny og endnu ikke gemt (Id er 0).
        /// </summary>
        public bool ErNy
        {
            get { return Aktivitet.Id == 0; }
        }

        /// <summary>
        /// Opretter siden med de repositories den skal bruge.
        /// </summary>
        /// <param name="aktivitetRepo">Repository til aktiviteter.</param>
        /// <param name="lederRepo">Repository til ledere.</param>
        /// <param name="tegnRepo">Repository til tegn.</param>
        /// <param name="tilmeldingRepo">Repository til tilmeldinger.</param>
        public RedigerModel(AktivitetRepository aktivitetRepo, LederRepository lederRepo,
            TegnRepository tegnRepo, TilmeldingRepository tilmeldingRepo)
        {
            _aktivitetRepo = aktivitetRepo;
            _lederRepo = lederRepo;
            _tegnRepo = tegnRepo;
            _tilmeldingRepo = tilmeldingRepo;
        }

        /// <summary>
        /// Viser formularen. Uden id udfyldes den med forslag til en ny aktivitet, og med id hentes den aktivitet der skal rettes. Kun ledere har adgang.
        /// </summary>
        /// <param name="id">Id på den aktivitet der skal rettes, eller null ved oprettelse.</param>
        /// <returns>Siden, en omdirigering til login hvis brugeren ikke er leder, eller NotFound hvis aktiviteten ikke findes.</returns>
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

        /// <summary>
        /// Validerer formularen og opretter eller opdaterer aktiviteten. Kun ledere har adgang.
        /// </summary>
        /// <returns>En omdirigering til oversigten når aktiviteten er gemt, siden igen ved valideringsfejl, en omdirigering til login hvis brugeren ikke er leder, eller NotFound hvis aktiviteten ikke findes.</returns>
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

            Aktivitet.Navn = Aktivitet.Navn.Trim();
            Aktivitet.Sted = Aktivitet.Sted.Trim();
            Aktivitet.Beskrivelse = Aktivitet.Beskrivelse.Trim();

            // Reglerne på Aktivitet-klassen ([Required], [Range], [StringLength])
            // er allerede tjekket af ASP.NET og ligger i ModelState.
            // Her tilføjes de regler der kræver mere end selve feltet:

            // En ny aktivitet skal ligge ude i fremtiden
            // (kun hvis feltet ellers er gyldigt, så man ikke får to fejl på samme felt)
            if (ErNy && ModelState.GetFieldValidationState("Aktivitet.Tidspunkt") != ModelValidationState.Invalid
                && Aktivitet.Tidspunkt < DateTime.Now)
            {
                ModelState.AddModelError("Aktivitet.Tidspunkt", "En ny aktivitet kan ikke ligge i fortiden");
            }

            // Mindst én leder, og de valgte ledere skal findes
            if (Aktivitet.LederIds.Count == 0)
            {
                ModelState.AddModelError("Aktivitet.LederIds", "Vælg mindst én leder der står for aktiviteten");
            }
            else if (Aktivitet.LederIds.Any(id => _lederRepo.HentEn(id) == null))
            {
                ModelState.AddModelError("Aktivitet.LederIds", "En af de valgte ledere findes ikke");
            }

            // De krævede tegn skal findes
            if (Aktivitet.KraevedeTegnIds.Any(id => _tegnRepo.HentEn(id) == null))
            {
                ModelState.AddModelError("Aktivitet.KraevedeTegnIds", "Et af de valgte tegn findes ikke");
            }

            // Ved rettelse: aktiviteten skal findes, og maks må ikke blive lavere end antal tilmeldte
            if (!ErNy)
            {
                if (_aktivitetRepo.HentEn(Aktivitet.Id) == null)
                {
                    return NotFound();
                }
                int tilmeldte = _tilmeldingRepo.HentForAktivitet(Aktivitet.Id).Count;
                if (Aktivitet.MaksDeltagere < tilmeldte)
                {
                    ModelState.AddModelError("Aktivitet.MaksDeltagere", "Der er allerede " + tilmeldte + " tilmeldt, så maks kan ikke være lavere");
                }
            }

            if (!ModelState.IsValid)
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
