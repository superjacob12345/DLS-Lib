using DLS_Lib.Repository;
using DLS_Lib.UserStories;
using DLS_Web.Hjaelpere;
using Microsoft.AspNetCore.Mvc;

namespace DLS_Web.Pages.Tegnene
{
    // Bruges både til at oprette (uden id) og rette (med id)
    public class RedigerModel : LederSide
    {
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
            Tegn.Navn = Tegn.Navn.Trim();
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
