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

        /// <summary>
        /// Listen af aktiviteter som vises på siden, sorteret efter tidspunkt.
        /// </summary>
        public List<Aktivitet> Aktiviteter { get; set; } = new List<Aktivitet>();

        /// <summary>
        /// Hvilke aktiviteter der vises: "alle" eller "mine". For ledere er "mine" dem lederen står for. For spejdere er det dem spejderen er tilmeldt eller kan tilmelde sig.
        /// </summary>
        [BindProperty(SupportsGet = true)]
        public string Vis { get; set; } = "alle";

        /// <summary>
        /// Angiver om brugeren der er logget ind, er en leder.
        /// </summary>
        public bool ErLeder { get; set; }

        /// <summary>
        /// Den spejder der er logget ind. Er null hvis det er en leder.
        /// </summary>
        public Spejder? MigSelv { get; set; }

        /// <summary>
        /// Opretter siden med de repositories den skal bruge.
        /// </summary>
        /// <param name="aktivitetRepo">Repository til aktiviteter.</param>
        /// <param name="tilmeldingRepo">Repository til tilmeldinger.</param>
        /// <param name="spejderRepo">Repository til spejdere.</param>
        /// <param name="lederRepo">Repository til ledere.</param>
        /// <param name="tegnRepo">Repository til tegn.</param>
        public IndexModel(AktivitetRepository aktivitetRepo, TilmeldingRepository tilmeldingRepo,
            SpejderRepository spejderRepo, LederRepository lederRepo, TegnRepository tegnRepo)
        {
            _aktivitetRepo = aktivitetRepo;
            _tilmeldingRepo = tilmeldingRepo;
            _spejderRepo = spejderRepo;
            _lederRepo = lederRepo;
            _tegnRepo = tegnRepo;
        }

        /// <summary>
        /// Viser aktiviteterne sorteret efter tidspunkt og filtreret efter Vis. Man skal være logget ind.
        /// </summary>
        /// <returns>Siden, eller en omdirigering til login hvis man ikke er logget ind.</returns>
        public IActionResult OnGet()
        {
            // Man skal være logget ind for at se aktiviteterne
            string? rolle = HttpContext.Session.GetString("Rolle");
            if (rolle == null)
            {
                return RedirectToPage("/Login");
            }

            int brugerId = HttpContext.Session.GetInt32("BrugerId") ?? 0;
            if (rolle == "Leder")
            {
                ErLeder = true;
            }
            else
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

        /// <summary>
        /// Tilmelder den spejder der er logget ind, til en aktivitet. Resultatet vises som en besked på siden.
        /// </summary>
        /// <param name="id">Id på aktiviteten.</param>
        /// <returns>En omdirigering tilbage til siden, eller til login hvis brugeren ikke er spejder.</returns>
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
                return RedirectToPage();
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
            return RedirectToPage();
        }

        /// <summary>
        /// Framelder den spejder der er logget ind, fra en aktivitet.
        /// </summary>
        /// <param name="id">Id på aktiviteten.</param>
        /// <returns>En omdirigering tilbage til siden, eller til login hvis brugeren ikke er spejder.</returns>
        public IActionResult OnPostFrameld(int id)
        {
            if (HttpContext.Session.GetString("Rolle") != "Spejder")
            {
                return RedirectToPage("/Login");
            }

            int spejderId = HttpContext.Session.GetInt32("BrugerId") ?? 0;
            _tilmeldingRepo.Frameld(spejderId, id);
            TempData["Besked"] = "Du er frameldt aktiviteten";
            return RedirectToPage();
        }

        /// <summary>
        /// Sletter en aktivitet og dens tilmeldinger. Kun ledere må slette.
        /// </summary>
        /// <param name="id">Id på aktiviteten.</param>
        /// <returns>En omdirigering tilbage til siden, eller til login hvis brugeren ikke er leder.</returns>
        public IActionResult OnPostSlet(int id)
        {
            if (HttpContext.Session.GetString("Rolle") != "Leder")
            {
                return RedirectToPage("/Login");
            }

            // Tilmeldingerne skal også væk, ellers peger de på en aktivitet der ikke findes
            _tilmeldingRepo.SletForAktivitet(id);
            _aktivitetRepo.Slet(id);
            TempData["Besked"] = "Aktiviteten er slettet";
            return RedirectToPage();
        }

        // ===== Hjælpemetoder som bruges i .cshtml =====

        /// <summary>
        /// Tæller hvor mange spejdere der er tilmeldt en aktivitet.
        /// </summary>
        /// <param name="aktivitet">Aktiviteten der skal tælles for.</param>
        /// <returns>Antallet af tilmeldte spejdere.</returns>
        public int AntalTilmeldte(Aktivitet aktivitet)
        {
            return _tilmeldingRepo.HentForAktivitet(aktivitet.Id).Count;
        }

        /// <summary>
        /// Tjekker om den spejder der er logget ind, er tilmeldt en aktivitet.
        /// </summary>
        /// <param name="aktivitet">Aktiviteten der skal tjekkes.</param>
        /// <returns>true hvis spejderen er tilmeldt. Altid false hvis det ikke er en spejder der er logget ind.</returns>
        public bool ErTilmeldt(Aktivitet aktivitet)
        {
            if (MigSelv == null)
            {
                return false;
            }
            return _tilmeldingRepo.ErTilmeldt(MigSelv.Id, aktivitet.Id);
        }

        /// <summary>
        /// Finder grunden til at den spejder der er logget ind, ikke kan tilmelde sig en aktivitet.
        /// </summary>
        /// <param name="aktivitet">Aktiviteten der skal tjekkes.</param>
        /// <returns>Grunden som tekst. Teksten er tom hvis spejderen kan tilmelde sig, eller hvis det ikke er en spejder der er logget ind.</returns>
        public string Grund(Aktivitet aktivitet)
        {
            if (MigSelv == null)
            {
                return "";
            }
            return _tilmeldingRepo.KanTilmelde(MigSelv, aktivitet);
        }

        /// <summary>
        /// Laver en kommasepareret tekst med navnene på en aktivitets ledere.
        /// </summary>
        /// <param name="aktivitet">Aktiviteten hvis ledere skal findes.</param>
        /// <returns>Ledernes navne adskilt af komma. Teksten er tom hvis der ingen ledere er.</returns>
        public string LederNavne(Aktivitet aktivitet)
        {
            string navne = "";
            foreach (int id in aktivitet.LederIds)
            {
                Leder? l = _lederRepo.HentEn(id);
                if (l != null)
                {
                    if (navne != "")
                    {
                        navne += ", ";
                    }
                    navne += l.Navn;
                }
            }
            return navne;
        }

        /// <summary>
        /// Laver en kommasepareret tekst med navnene på de tegn en aktivitet kræver.
        /// </summary>
        /// <param name="aktivitet">Aktiviteten hvis krævede tegn skal findes.</param>
        /// <returns>Tegnenes navne adskilt af komma, eller "Ingen" hvis aktiviteten ikke kræver tegn.</returns>
        public string TegnNavne(Aktivitet aktivitet)
        {
            string navne = "";
            foreach (int id in aktivitet.KraevedeTegnIds)
            {
                Tegn? t = _tegnRepo.HentEn(id);
                if (t != null)
                {
                    if (navne != "")
                    {
                        navne += ", ";
                    }
                    navne += t.Navn;
                }
            }
            if (navne == "")
            {
                return "Ingen";
            }
            return navne;
        }
    }
}
