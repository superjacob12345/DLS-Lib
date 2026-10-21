using DLS_Lib.Repository;
using DLS_Lib.UserStories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DLS_Web.Pages.Spejdere
{
    public class RedigerModel : PageModel
    {
        private SpejderRepository _repo;

        [BindProperty]
        public Spejder Spejder { get; set; } = new Spejder();

        public RedigerModel(SpejderRepository repo)
        {
            _repo = repo;
        }

        public IActionResult OnGet(int id)
        {
            Spejder? spejder = _repo.HentEn(id);
            if (spejder == null)
            {
                return NotFound();
            }

            Spejder = spejder;
            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _repo.Opdater(Spejder);
            return RedirectToPage("Index");
        }
    }
}
