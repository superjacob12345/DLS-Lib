using DLS_Lib.Repository;
using DLS_Lib.UserStories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DLS_Web.Pages.Ledere
{
    public class IndexModel : PageModel
    {
        private LederRepository _repo;
        private AktivitetRepository _aktivitetRepo;

        /// <summary>
        /// Listen af ledere som vises på siden, sorteret efter navn.
        /// </summary>
        public List<Leder> Ledere { get; set; } = new List<Leder>();

        /// <summary>
        /// Id på den leder der er logget ind. En leder kan ikke slette sig selv.
        /// </summary>
        public int MitId { get; set; }

        /// <summary>
        /// Opretter siden med de repositories den skal bruge.
        /// </summary>
        /// <param name="repo">Repository til ledere.</param>
        /// <param name="aktivitetRepo">Repository til aktiviteter.</param>
        public IndexModel(LederRepository repo, AktivitetRepository aktivitetRepo)
        {
            _repo = repo;
            _aktivitetRepo = aktivitetRepo;
        }

        /// <summary>
        /// Viser listen af ledere. Kun ledere har adgang.
        /// </summary>
        /// <returns>Siden, eller en omdirigering til login hvis brugeren ikke er leder.</returns>
        public IActionResult OnGet()
        {
            // Kun ledere må se siden
            if (HttpContext.Session.GetString("Rolle") != "Leder")
            {
                return RedirectToPage("/Login");
            }

            Ledere = _repo.HentAlle().OrderBy(l => l.Navn).ToList();
            MitId = HttpContext.Session.GetInt32("BrugerId") ?? 0;
            return Page();
        }

        /// <summary>
        /// Sletter en leder. En leder kan ikke slette sig selv eller en leder der er eneste leder på en aktivitet.
        /// </summary>
        /// <param name="id">Id på den leder der skal slettes.</param>
        /// <returns>En omdirigering tilbage til siden, eller til login hvis brugeren ikke er leder.</returns>
        public IActionResult OnPostSlet(int id)
        {
            if (HttpContext.Session.GetString("Rolle") != "Leder")
            {
                return RedirectToPage("/Login");
            }

            Leder? leder = _repo.HentEn(id);
            if (leder == null)
            {
                return RedirectToPage();
            }

            if (id == HttpContext.Session.GetInt32("BrugerId"))
            {
                TempData["Fejl"] = "Du kan ikke slette dig selv";
                return RedirectToPage();
            }

            // En aktivitet skal have mindst én leder
            foreach (Aktivitet a in _aktivitetRepo.HentAlle())
            {
                if (a.LederIds.Contains(id) && a.LederIds.Count == 1)
                {
                    TempData["Fejl"] = leder.Navn + " er eneste leder på \"" + a.Navn + "\". Giv aktiviteten en anden leder først.";
                    return RedirectToPage();
                }
            }

            _repo.Slet(id);
            TempData["Besked"] = leder.Navn + " er slettet";
            return RedirectToPage();
        }

        /// <summary>
        /// Laver en kommasepareret tekst med navnene på de aktiviteter en leder står for.
        /// </summary>
        /// <param name="leder">Lederen hvis aktiviteter skal findes.</param>
        /// <returns>Aktiviteternes navne adskilt af komma. Teksten er tom hvis lederen ikke står for nogen aktiviteter.</returns>
        public string AktivitetNavne(Leder leder)
        {
            string navne = "";
            foreach (Aktivitet a in _aktivitetRepo.HentAlle())
            {
                if (a.LederIds.Contains(leder.Id))
                {
                    if (navne != "")
                    {
                        navne += ", ";
                    }
                    navne += a.Navn;
                }
            }
            return navne;
        }
    }
}
