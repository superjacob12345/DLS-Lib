//using DLS_Lib.Repository;
//using DLS_Lib.UserStories;
//using DLS_Web.Hjaelpere;
//using Microsoft.AspNetCore.Mvc;

//namespace DLS_Web.Pages.Aktiviteter
//{
//    // Både spejdere og ledere bruger denne side. Spejdere kan tilmelde/framelde,
//    // ledere kan rette, slette og se deltagere.
//    public class IndexModel : BeskyttetSide
//    {
//        private AktivitetRepository _aktivitetRepo;
//        private TilmeldingRepository _tilmeldingRepo;
//        private SpejderRepository _spejderRepo;
//        private LederRepository _lederRepo;
//        private TegnRepository _tegnRepo;

//        // Listen som vises på siden, sorteret efter tidspunkt
//        public List<Aktivitet> Aktiviteter { get; set; } = new List<Aktivitet>();

//        // "alle" eller "mine" (ledere: dem jeg står for, spejdere: dem jeg kan deltage i)
//        [BindProperty(SupportsGet = true)]
//        public string Vis { get; set; } = "alle";

//        public bool ErLeder { get; set; }

//        // Den spejder der er logget ind (null hvis det er en leder)
//        public Spejder? MigSelv { get; set; }

//        public IndexModel(AktivitetRepository aktivitetRepo, TilmeldingRepository tilmeldingRepo,
//            SpejderRepository spejderRepo, LederRepository lederRepo, TegnRepository tegnRepo)
//        {
//            _aktivitetRepo = aktivitetRepo;
//            _tilmeldingRepo = tilmeldingRepo;
//            _spejderRepo = spejderRepo;
//            _lederRepo = lederRepo;
//            _tegnRepo = tegnRepo;
//        }

//        public void OnGet()
//        {
//            ErLeder = Bruger.ErLeder(HttpContext.Session);
//            int brugerId = Bruger.Id(HttpContext.Session);
//            if (!ErLeder)
//            {
//                MigSelv = _spejderRepo.HentEn(brugerId);
//            }

//            foreach (Aktivitet a in _aktivitetRepo.HentAlle().OrderBy(a => a.Tidspunkt))
//            {
//                if (Vis == "mine")
//                {
//                    if (ErLeder && !a.LederIds.Contains(brugerId))
//                    {
//                        continue;
//                    }
//                    if (MigSelv != null && !ErTilmeldt(a) && Grund(a) != "")
//                    {
//                        continue;
//                    }
//                }
//                Aktiviteter.Add(a);
//            }
//        }

//        public IActionResult OnPostTilmeld(int id)
//        {
//            Spejder? spejder = HentIndloggetSpejder();
//            Aktivitet? aktivitet = _aktivitetRepo.HentEn(id);
//            if (spejder == null || aktivitet == null)
//            {
//                TempData["Fejl"] = "Kun spejdere kan tilmelde sig aktiviteter";
//                return RedirectToPage(new { Vis });
//            }

//            string fejl = _tilmeldingRepo.Tilmeld(spejder, aktivitet);
//            if (fejl == "")
//            {
//                TempData["Besked"] = "Du er tilmeldt " + aktivitet.Navn;
//            }
//            else
//            {
//                TempData["Fejl"] = fejl;
//            }
//            return RedirectToPage(new { Vis });
//        }

//        public IActionResult OnPostFrameld(int id)
//        {
//            Spejder? spejder = HentIndloggetSpejder();
//            Aktivitet? aktivitet = _aktivitetRepo.HentEn(id);
//            if (spejder != null && aktivitet != null)
//            {
//                _tilmeldingRepo.Frameld(spejder.Id, aktivitet.Id);
//                TempData["Besked"] = "Du er frameldt " + aktivitet.Navn;
//            }
//            return RedirectToPage(new { Vis });
//        }

//        public IActionResult OnPostSlet(int id)
//        {
//            if (!Bruger.ErLeder(HttpContext.Session))
//            {
//                TempData["Fejl"] = "Kun ledere kan slette aktiviteter";
//                return RedirectToPage(new { Vis });
//            }

//            Aktivitet? aktivitet = _aktivitetRepo.HentEn(id);
//            if (aktivitet != null)
//            {
//                // Tilmeldingerne skal også væk, ellers peger de på en aktivitet der ikke findes
//                _tilmeldingRepo.SletForAktivitet(id);
//                _aktivitetRepo.Slet(id);
//                TempData["Besked"] = aktivitet.Navn + " er nedlagt";
//            }
//            return RedirectToPage(new { Vis });
//        }

//        // ===== Hjælpemetoder som bruges i .cshtml =====

//        public int AntalTilmeldte(Aktivitet aktivitet)
//        {
//            return _tilmeldingRepo.HentForAktivitet(aktivitet.Id).Count;
//        }

//        public bool ErTilmeldt(Aktivitet aktivitet)
//        {
//            if (MigSelv == null)
//            {
//                return false;
//            }
//            return _tilmeldingRepo.ErTilmeldt(MigSelv.Id, aktivitet.Id);
//        }

//        // Hvorfor spejderen ikke kan tilmelde sig ("" = det kan godt lade sig gøre)
//        public string Grund(Aktivitet aktivitet)
//        {
//            if (MigSelv == null)
//            {
//                return "";
//            }
//            return _tilmeldingRepo.KanTilmelde(MigSelv, aktivitet);
//        }

//        public string LederNavne(Aktivitet aktivitet)
//        {
//            List<string> navne = new List<string>();
//            foreach (int id in aktivitet.LederIds)
//            {
//                Leder? l = _lederRepo.HentEn(id);
//                if (l != null)
//                {
//                    navne.Add(l.Navn);
//                }
//            }
//            if (navne.Count == 0)
//            {
//                return "Ingen leder endnu";
//            }
//            return string.Join(", ", navne);
//        }

//        public List<Tegn> KraevedeTegn(Aktivitet aktivitet)
//        {
//            List<Tegn> tegn = new List<Tegn>();
//            foreach (int id in aktivitet.KraevedeTegnIds)
//            {
//                Tegn? t = _tegnRepo.HentEn(id);
//                if (t != null)
//                {
//                    tegn.Add(t);
//                }
//            }
//            return tegn;
//        }

//        private Spejder? HentIndloggetSpejder()
//        {
//            if (!Bruger.ErSpejder(HttpContext.Session))
//            {
//                return null;
//            }
//            return _spejderRepo.HentEn(Bruger.Id(HttpContext.Session));
//        }
//    }
//}
