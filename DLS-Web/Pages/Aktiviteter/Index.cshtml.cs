using DLS_Lib.Repository;
using DLS_Lib.UserStories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DLS_Web.Pages.Aktiviteter
{
    public class IndexModel : PageModel
    {
        // Repository'et hvor aktiviteterne ligger
        private AktivitetRepository _repo;

        // Listen som vises på siden
        public List<Aktivitet> Aktiviteter { get; set; } = new List<Aktivitet>();

        // Repository'et kommer automatisk fra Program.cs
        public IndexModel(AktivitetRepository repo)
        {
            _repo = repo;
        }

        // Kører når siden åbnes
        public void OnGet()
        {
            Aktiviteter = _repo.HentAlle();
        }
    }
}
