using DLS_Lib.Repository;
using DLS_Lib.UserStories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DLS_Web.Pages.Spejdere
{
    public class OpretModel : PageModel
    {
        private SpejderRepository _repo;

        [BindProperty]
        public Spejder Spejder { get; set; } = new Spejder();

        public OpretModel(SpejderRepository repo)
        {
            _repo = repo;
        }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _repo.Opret(Spejder);
            return RedirectToPage("Index");
        }
    }
}
