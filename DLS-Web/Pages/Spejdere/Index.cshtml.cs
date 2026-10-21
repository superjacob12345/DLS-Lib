using DLS_Lib.Repository;
using DLS_Lib.UserStories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DLS_Web.Pages.Spejdere
{
    public class IndexModel : PageModel
    {
        private SpejderRepository _repo;

        // Listen som vises på siden
        public List<Spejder> Spejdere { get; set; } = new List<Spejder>();

        public IndexModel(SpejderRepository repo)
        {
            _repo = repo;
        }

        public void OnGet()
        {
            Spejdere = _repo.HentAlle();
        }

        public IActionResult OnPostSlet(int id)
        {
            _repo.Slet(id);
            return RedirectToPage();
        }
    }
}
