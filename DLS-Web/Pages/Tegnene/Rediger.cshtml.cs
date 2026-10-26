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

        [BindProperty]
        public Tegn Tegn { get; set; } = new Tegn();

        // Fejlbeskeder der vises over formularen
        public List<string> Fejl { get; set; } = new List<string>();

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
