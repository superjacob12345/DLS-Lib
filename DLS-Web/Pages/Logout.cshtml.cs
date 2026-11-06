using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DLS_Web.Pages
{
    public class LogoutModel : PageModel
    {
        /// <summary>
        /// Logger brugeren ud på samme måde som OnPost.
        /// </summary>
        /// <returns>En omdirigering til forsiden.</returns>
        public IActionResult OnGet()
        {
            return OnPost();
        }

        /// <summary>
        /// Logger brugeren ud ved at tømme sessionen.
        /// </summary>
        /// <returns>En omdirigering til forsiden.</returns>
        public IActionResult OnPost()
        {
            HttpContext.Session.Clear();
            TempData["Besked"] = "Du er nu logget ud";
            return RedirectToPage("/Index");
        }
    }
}
