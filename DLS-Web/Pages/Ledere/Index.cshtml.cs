using DLS_Lib.Repository;
using DLS_Lib.UserStories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DLS_Web.Pages.Ledere
{
    public class IndexModel : PageModel
    {
        private LederRepository _repo;

        // Listen som vises på siden
        public List<Leder> Ledere { get; set; } = new List<Leder>();

        public IndexModel(LederRepository repo)
        {
            _repo = repo;
        }

        public void OnGet()
        {
            Ledere = _repo.HentAlle();
        }

        public IActionResult OnPostSlet(int id)
        {
            _repo.Slet(id);
            return RedirectToPage();
        }
    }
}
