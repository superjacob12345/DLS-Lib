using DLS_Lib.Repository;
using DLS_Lib.UserStories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DLS_Web.Pages.Ledere
{
    public class OpretModel : PageModel
    {
        private LederRepository _repo;

        [BindProperty]
        public Leder Leder { get; set; } = new Leder();

        public OpretModel(LederRepository repo)
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

            _repo.Opret(Leder);
            return RedirectToPage("Index");
        }
    }
}
