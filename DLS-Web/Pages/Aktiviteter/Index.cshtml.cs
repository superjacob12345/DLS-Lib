using DLS_Lib.Repository;
using DLS_Lib.UserStories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DLS_Web.Pages.Aktiviteter
{
    // Både spejdere og ledere bruger denne side. Spejdere kan tilmelde/framelde,
    // ledere kan rette, slette og se hvem der er tilmeldt.
    public class IndexModel : PageModel
    {
        private AktivitetRepository _aktivitetRepo;
        private TilmeldingRepository _tilmeldingRepo;
        private SpejderRepository _spejderRepo;
        private LederRepository _lederRepo;
        private TegnRepository _tegnRepo;

        // Listen som vises på siden, sorteret efter tidspunkt
        public List<Aktivitet> Aktiviteter { get; set; } = new List<Aktivitet>();

        // "alle" eller "mine" (ledere: dem jeg står for, spejdere: dem jeg kan deltage i)
        [BindProperty(SupportsGet = true)]
        public string Vis { get; set; } = "alle";

        public bool ErLeder { get; set; }

        // Den spejder der er logget ind (null hvis det er en leder)
        public Spejder? MigSelv { get; set; }

        public IndexModel(AktivitetRepository aktivitetRepo, TilmeldingRepository tilmeldingRepo,
            SpejderRepository spejderRepo, LederRepository lederRepo, TegnRepository tegnRepo)
        {
            _aktivitetRepo = aktivitetRepo;
            _tilmeldingRepo = tilmeldingRepo;
            _spejderRepo = spejderRepo;
            _lederRepo = lederRepo;
            _tegnRepo = tegnRepo;
        }

        public IActionResult OnGet()
        {
            // Man skal være logget ind for at se aktiviteterne
            string? rolle = HttpContext.Session.GetString("Rolle");
            if (rolle == null)
            {
                return RedirectToPage("/Login");
            }

            int brugerId = HttpContext.Session.GetInt32("BrugerId") ?? 0;
            ErLeder = rolle == "Leder";
            if (!ErLeder)
            {
                MigSelv = _spejderRepo.HentEn(brugerId);
            }

            List<Aktivitet> sorteret = _aktivitetRepo.HentAlle().OrderBy(a => a.Tidspunkt).ToList();
            foreach (Aktivitet a in sorteret)
            {
                if (Vis == "mine")
                {
                    // Lederen ser kun dem lederen står for
                    if (ErLeder && !a.LederIds.Contains(brugerId))
                    {
                        continue;
                    }
                    // Spejderen ser kun dem spejderen er tilmeldt eller kan tilmelde sig
                    if (MigSelv != null && !ErTilmeldt(a) && Grund(a) != "")
                    {
                        continue;
                    }
                }
                Aktiviteter.Add(a);
            }
            return Page();
        }

        public IActionResult OnPostTilmeld(int id)
        {
            if (HttpContext.Session.GetString("Rolle") != "Spejder")
            {
                return RedirectToPage("/Login");
            }

            Spejder? spejder = _spejderRepo.HentEn(HttpContext.Session.GetInt32("BrugerId") ?? 0);
            Aktivitet? aktivitet = _aktivitetRepo.HentEn(id);
            if (spejder == null || aktivitet == null)
            {
                return RedirectToPage(new { Vis });
            }

            string fejl = _tilmeldingRepo.Tilmeld(spejder, aktivitet);
            if (fejl == "")
            {
                TempData["Besked"] = "Du er tilmeldt " + aktivitet.Navn;
            }
            else
            {
                TempData["Fejl"] = fejl;
            }
            return RedirectToPage(new { Vis });
        }

        public IActionResult OnPostFrameld(int id)
        {
            if (HttpContext.Session.GetString("Rolle") != "Spejder")
            {
                return RedirectToPage("/Login");
            }

            int spejderId = HttpContext.Session.GetInt32("BrugerId") ?? 0;
            Aktivitet? aktivitet = _aktivitetRepo.HentEn(id);
            if (aktivitet != null)
            {
                _tilmeldingRepo.Frameld(spejderId, aktivitet.Id);
                TempData["Besked"] = "Du er frameldt " + aktivitet.Navn;
            }
            return RedirectToPage(new { Vis });
        }

        public IActionResult OnPostSlet(int id)
        {
            if (HttpContext.Session.GetString("Rolle") != "Leder")
            {
                return RedirectToPage("/Login");
            }

            Aktivitet? aktivitet = _aktivitetRepo.HentEn(id);
            if (aktivitet != null)
            {
                // Tilmeldingerne skal også væk, ellers peger de på en aktivitet der ikke findes
                _tilmeldingRepo.SletForAktivitet(id);
                _aktivitetRepo.Slet(id);
                TempData["Besked"] = aktivitet.Navn + " er nedlagt";
            }
            return RedirectToPage(new { Vis });
        }

        // ===== Hjælpemetoder som bruges i .cshtml =====

        public int AntalTilmeldte(Aktivitet aktivitet)
        {
            return _tilmeldingRepo.HentForAktivitet(aktivitet.Id).Count;
        }

        public bool ErTilmeldt(Aktivitet aktivitet)
        {
            if (MigSelv == null)
            {
                return false;
            }
            return _tilmeldingRepo.ErTilmeldt(MigSelv.Id, aktivitet.Id);
        }

        // Hvorfor spejderen ikke kan tilmelde sig ("" = det kan godt lade sig gøre)
        public string Grund(Aktivitet aktivitet)
        {
            if (MigSelv == null)
            {
                return "";
            }
            return _tilmeldingRepo.KanTilmelde(MigSelv, aktivitet);
        }

        public string LederNavne(Aktivitet aktivitet)
        {
            List<string> navne = new List<string>();
            foreach (int id in aktivitet.LederIds)
            {
                Leder? l = _lederRepo.HentEn(id);
                if (l != null)
                {
                    navne.Add(l.Navn);
                }
            }
            if (navne.Count == 0)
            {
                return "Ingen leder endnu";
            }
            return string.Join(", ", navne);
        }

        public List<Tegn> KraevedeTegn(Aktivitet aktivitet)
        {
            List<Tegn> tegn = new List<Tegn>();
            foreach (int id in aktivitet.KraevedeTegnIds)
            {
                Tegn? t = _tegnRepo.HentEn(id);
                if (t != null)
                {
                    tegn.Add(t);
                }
            }
            return tegn;
        }
    }
}
