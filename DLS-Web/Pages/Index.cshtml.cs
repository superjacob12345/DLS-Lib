//using DLS_Lib.Repository;
//using DLS_Lib.UserStories;
//using DLS_Web.Hjaelpere;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.RazorPages;

//namespace DLS_Web.Pages
//{
//    public class IndexModel : PageModel
//    {
//        private AktivitetRepository _aktivitetRepo;
//        private SpejderRepository _spejderRepo;
//        private TilmeldingRepository _tilmeldingRepo;

//        // De næste aktiviteter der vises som "program"
//        public List<Aktivitet> Kommende { get; set; } = new List<Aktivitet>();

//        public int AntalAktiviteter { get; set; }
//        public int AntalSpejdere { get; set; }
//        public int AntalTilmeldinger { get; set; }

//        // Hvor mange aktiviteter den indloggede spejder er tilmeldt
//        public int MineTilmeldinger { get; set; }

//        public IndexModel(AktivitetRepository aktivitetRepo, SpejderRepository spejderRepo, TilmeldingRepository tilmeldingRepo)
//        {
//            _aktivitetRepo = aktivitetRepo;
//            _spejderRepo = spejderRepo;
//            _tilmeldingRepo = tilmeldingRepo;
//        }

//        public void OnGet()
//        {
//            Kommende = _aktivitetRepo.HentAlle()
//                .Where(a => a.Tidspunkt >= DateTime.Now)
//                .OrderBy(a => a.Tidspunkt)
//                .Take(4)
//                .ToList();

//            AntalAktiviteter = _aktivitetRepo.HentAlle().Count;
//            AntalSpejdere = _spejderRepo.HentAlle().Count;
//            AntalTilmeldinger = _tilmeldingRepo.HentAlle().Count;

//            if (Bruger.ErSpejder(HttpContext.Session))
//            {
//                MineTilmeldinger = _tilmeldingRepo.HentForSpejder(Bruger.Id(HttpContext.Session)).Count;
//            }
//        }
//    }
//}
