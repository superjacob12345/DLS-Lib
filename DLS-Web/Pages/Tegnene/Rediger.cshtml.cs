using DLS_Lib.Repository;
using DLS_Lib.UserStories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DLS_Web.Pages.Tegnene
{
    // Bruges både til at oprette (uden id) og rette (med id)
    public class RedigerModel : PageModel
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

        private TegnRepository _repo;

        [BindProperty]
        public Tegn Tegn { get; set; } = new Tegn();

        public bool ErNy
        {
            get { return Tegn.Id == 0; }
        }

        public RedigerModel(TegnRepository repo)
        {
            _repo = repo;
        }

        public IActionResult OnGet(int? id)
        {
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

        public IActionResult OnPost()
        {
            // Tomme felter kommer ind som null, så de laves om til ""
            Tegn.Navn = (Tegn.Navn ?? "").Trim();
            if (Tegn.Navn == "")
            {
                ModelState.AddModelError("", "Tegnet skal have et navn");
            }

            if (!ModelState.IsValid)
            {
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
