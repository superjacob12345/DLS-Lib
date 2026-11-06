using DLS_Lib.Repository;
using DLS_Lib.UserStories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DLS_Web.Pages
{
    public class IndexModel : PageModel
    {
        private AktivitetRepository _aktivitetRepo;

        /// <summary>
        /// De næste aktiviteter der vises på forsiden. Højst 4, og kun aktiviteter der ikke er begyndt endnu.
        /// </summary>
        public List<Aktivitet> Kommende { get; set; } = new List<Aktivitet>();

        /// <summary>
        /// Opretter siden med det repository den skal bruge.
        /// </summary>
        /// <param name="aktivitetRepo">Repository til aktiviteter.</param>
        public IndexModel(AktivitetRepository aktivitetRepo)
        {
            _aktivitetRepo = aktivitetRepo;
        }

        /// <summary>
        /// Viser forsiden og finder de 4 første aktiviteter der ikke er begyndt endnu.
        /// </summary>
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
