using DLS_Lib.Repository;
using DLS_Lib.UserStories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DLS_Web.Pages
{
    public class IndexModel : PageModel
    {
        private AktivitetRepository _aktivitetRepo;

        // De næste aktiviteter der vises på forsiden
        public List<Aktivitet> Kommende { get; set; } = new List<Aktivitet>();

        public IndexModel(AktivitetRepository aktivitetRepo)
        {
            _aktivitetRepo = aktivitetRepo;
        }

        public void OnGet()
        {
            // Find de 4 første aktiviteter der ikke er begyndt endnu
            List<Aktivitet> sorteret = _aktivitetRepo.HentAlle().OrderBy(a => a.Tidspunkt).ToList();
            foreach (Aktivitet a in sorteret)
            {
                if (a.Tidspunkt >= DateTime.Now && Kommende.Count < 4)
                {
                    Kommende.Add(a);
                }
            }
        }
    }
}
