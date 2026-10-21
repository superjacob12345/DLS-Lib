using DLS_Lib.Repository;
using DLS_Lib.UserStories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DLS_Web.Pages.Ledere
{
    public class RedigerModel : PageModel
    {
        private LederRepository _repo;

        [BindProperty]
        public Leder Leder { get; set; } = new Leder();

        public RedigerModel(LederRepository repo)
        {
            _repo = repo;
        }

        public IActionResult OnGet(int id)
        {
            Leder? leder = _repo.HentEn(id);
            if (leder == null)
            {
                return NotFound();
            }

            Leder = leder;
            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _repo.Opdater(Leder);
            return RedirectToPage("Index");
        }
    }
}
