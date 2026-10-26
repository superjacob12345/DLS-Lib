using DLS_Lib.Repository;
using DLS_Lib.UserStories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DLS_Web.Pages
{
    public class IndexModel : PageModel
    {
        private AktivitetRepository _aktivitetRepo;
        private SpejderRepository _spejderRepo;
        private TilmeldingRepository _tilmeldingRepo;

        // De næste aktiviteter der vises som "program"
        public List<Aktivitet> Kommende { get; set; } = new List<Aktivitet>();

        public int AntalAktiviteter { get; set; }
        public int AntalSpejdere { get; set; }
        public int AntalTilmeldinger { get; set; }

        // Hvor mange aktiviteter den indloggede spejder er tilmeldt
        public int MineTilmeldinger { get; set; }

        public IndexModel(AktivitetRepository aktivitetRepo, SpejderRepository spejderRepo, TilmeldingRepository tilmeldingRepo)
        {
            _aktivitetRepo = aktivitetRepo;
            _spejderRepo = spejderRepo;
            _tilmeldingRepo = tilmeldingRepo;
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

            AntalAktiviteter = _aktivitetRepo.HentAlle().Count;
            AntalSpejdere = _spejderRepo.HentAlle().Count;
            AntalTilmeldinger = _tilmeldingRepo.HentAlle().Count;

            if (HttpContext.Session.GetString("Rolle") == "Spejder")
            {
                int spejderId = HttpContext.Session.GetInt32("BrugerId") ?? 0;
                MineTilmeldinger = _tilmeldingRepo.HentForSpejder(spejderId).Count;
            }
        }
    }
}
