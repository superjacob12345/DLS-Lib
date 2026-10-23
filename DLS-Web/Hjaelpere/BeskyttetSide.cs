using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DLS_Web.Hjaelpere
{
    // Sider der arver fra denne klasse kræver at man er logget ind.
    // Kører før OnGet/OnPost, så siderne selv slipper for at tjekke.
    public class BeskyttetSide : PageModel
    {
        // null = alle der er logget ind må se siden
        protected virtual string? KraevetRolle
        {
            get { return null; }
        }

        public override void OnPageHandlerExecuting(PageHandlerExecutingContext context)
        {
            string? rolle = Bruger.Rolle(HttpContext.Session);

            if (rolle == null)
            {
                TempData["Fejl"] = "Du skal logge ind først";
                context.Result = RedirectToPage("/Login");
                return;
            }

            if (KraevetRolle != null && rolle != KraevetRolle)
            {
                TempData["Fejl"] = "Den side er kun for " + KraevetRolle.ToLower() + "e";
                context.Result = RedirectToPage("/Index");
            }
        }
    }

    // Kun ledere har adgang
    public class LederSide : BeskyttetSide
    {
        protected override string? KraevetRolle
        {
            get { return Bruger.Leder; }
        }
    }

    // Kun spejdere har adgang
    public class SpejderSide : BeskyttetSide
    {
        protected override string? KraevetRolle
        {
            get { return Bruger.Spejder; }
        }
    }
}
