using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DLS_Web.Pages
{
    public class LogoutModel : PageModel
    {
        public IActionResult OnGet()
        {
            return OnPost();
        }

        public IActionResult OnPost()
        {
            HttpContext.Session.Clear();
            TempData["Besked"] = "Du er nu logget ud";
            return RedirectToPage("/Index");
        }
    }
}
