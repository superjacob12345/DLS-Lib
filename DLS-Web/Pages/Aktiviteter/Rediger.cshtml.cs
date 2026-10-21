using DLS_Lib.Repository;
using DLS_Lib.UserStories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DLS_Web.Pages.Aktiviteter
{
    public class RedigerModel : PageModel
    {
        private AktivitetRepository _repo;

        [BindProperty]
        public Aktivitet Aktivitet { get; set; } = new Aktivitet();

        public RedigerModel(AktivitetRepository repo)
        {
            _repo = repo;
        }

        // Henter aktiviteten så formularen kan udfyldes
        public IActionResult OnGet(int id)
        {
            Aktivitet? aktivitet = _repo.HentEn(id);
            if (aktivitet == null)
            {
                return NotFound();
            }

            Aktivitet = aktivitet;
            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _repo.Opdater(Aktivitet);
            return RedirectToPage("Index");
        }
    }
}
