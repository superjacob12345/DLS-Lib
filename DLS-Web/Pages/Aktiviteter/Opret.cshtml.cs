using DLS_Lib.Repository;
using DLS_Lib.UserStories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DLS_Web.Pages.Aktiviteter
{
    public class OpretModel : PageModel
    {
        private AktivitetRepository _repo;

        // Udfyldes automatisk med det der står i formularen
        [BindProperty]
        public Aktivitet Aktivitet { get; set; } = new Aktivitet();

        public OpretModel(AktivitetRepository repo)
        {
            _repo = repo;
        }

        public void OnGet()
        {
            Aktivitet.Tidspunkt = DateTime.Today.AddHours(12);
        }

        // Kører når formularen sendes
        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _repo.Opret(Aktivitet);
            return RedirectToPage("Index");
        }
    }
}
