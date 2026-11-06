using DLS_Lib.Repository;
using DLS_Lib.UserStories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DLS_Web.Pages.Tegnene
{
    // Bruges både til at oprette (uden id) og rette (med id)
    public class RedigerModel : PageModel
    {
        private TegnRepository _repo;

        /// <summary>
        /// Det tegn der oprettes eller rettes i formularen.
        /// </summary>
        [BindProperty]
        public Tegn Tegn { get; set; } = new Tegn();

        /// <summary>
        /// Fejlbeskeder der vises over formularen.
        /// </summary>
        public List<string> Fejl { get; set; } = new List<string>();

        /// <summary>
        /// Angiver om tegnet er nyt og endnu ikke gemt (Id er 0).
        /// </summary>
        public bool ErNy
        {
            get { return Tegn.Id == 0; }
        }

        /// <summary>
        /// Opretter siden med det repository den skal bruge.
        /// </summary>
        /// <param name="repo">Repository til tegn.</param>
        public RedigerModel(TegnRepository repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Viser formularen. Uden id er den tom, og med id hentes det tegn der skal rettes. Kun ledere har adgang.
        /// </summary>
        /// <param name="id">Id på det tegn der skal rettes, eller null ved oprettelse.</param>
        /// <returns>Siden, en omdirigering til login hvis brugeren ikke er leder, eller NotFound hvis tegnet ikke findes.</returns>
        public IActionResult OnGet(int? id)
        {
            // Kun ledere må oprette og rette tegn
            if (HttpContext.Session.GetString("Rolle") != "Leder")
            {
                return RedirectToPage("/Login");
            }

            if (id != null)
            {
                Tegn? tegn = _repo.HentEn(id.Value);
                if (tegn == null)
                {
                    return NotFound();
                }
                Tegn = tegn;
            }
            return Page();
        }

        /// <summary>
        /// Validerer formularen og opretter eller opdaterer tegnet. Kun ledere har adgang.
        /// </summary>
        /// <returns>En omdirigering til oversigten når tegnet er gemt, siden igen ved valideringsfejl, eller en omdirigering til login hvis brugeren ikke er leder.</returns>
        public IActionResult OnPost()
        {
            if (HttpContext.Session.GetString("Rolle") != "Leder")
            {
                return RedirectToPage("/Login");
            }

            // Et tomt tekstfelt kommer ind som null, så det laves om til ""
            if (Tegn.Navn == null)
            {
                Tegn.Navn = "";
            }
            Tegn.Navn = Tegn.Navn.Trim();

            if (Tegn.Navn == "")
            {
                Fejl.Add("Tegnet skal have et navn");
                return Page();
            }

            if (Tegn.Id == 0)
            {
                _repo.Opret(Tegn);
                TempData["Besked"] = Tegn.Navn + " er oprettet";
            }
            else
            {
                _repo.Opdater(Tegn);
                TempData["Besked"] = Tegn.Navn + " er gemt";
            }
            return RedirectToPage("Index");
        }
    }
}
