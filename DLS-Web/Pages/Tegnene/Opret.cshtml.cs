using DLS_Lib.Repository;
using DLS_Lib.UserStories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DLS_Web.Pages.Tegnene
{
    public class OpretModel : PageModel
    {
        private TegnRepository _repo;

        [BindProperty]
        public Tegn Tegn { get; set; } = new Tegn();

        public OpretModel(TegnRepository repo)
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

            _repo.Opret(Tegn);
            return RedirectToPage("Index");
        }
    }
}
