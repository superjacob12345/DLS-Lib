using DLS_Lib.Repository;
using DLS_Lib.UserStories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DLS_Web.Pages.Tegnene
{
    public class RedigerModel : PageModel
    {
        private TegnRepository _repo;

        [BindProperty]
        public Tegn Tegn { get; set; } = new Tegn();

        public RedigerModel(TegnRepository repo)
        {
            _repo = repo;
        }

        public IActionResult OnGet(int id)
        {
            Tegn? tegn = _repo.HentEn(id);
            if (tegn == null)
            {
                return NotFound();
            }

            Tegn = tegn;
            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _repo.Opdater(Tegn);
            return RedirectToPage("Index");
        }
    }
}
