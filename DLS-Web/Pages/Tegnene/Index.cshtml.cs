using DLS_Lib.Repository;
using DLS_Lib.UserStories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

// Mappen hedder "Tegnene", fordi et namespace der hedder "Tegn" ville blive forvekslet med klassen Tegn
namespace DLS_Web.Pages.Tegnene
{
    public class IndexModel : PageModel
    {
        private TegnRepository _repo;

        // Listen som vises på siden
        public List<Tegn> Tegn { get; set; } = new List<Tegn>();

        public IndexModel(TegnRepository repo)
        {
            _repo = repo;
        }

        public void OnGet()
        {
            Tegn = _repo.HentAlle();
        }

        public IActionResult OnPostSlet(int id)
        {
            _repo.Slet(id);
            return RedirectToPage();
        }
    }
}
