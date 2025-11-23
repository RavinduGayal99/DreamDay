using Microsoft.AspNetCore.Mvc;

namespace DreamDay.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            // This is the single point of truth for redirection after login.
            if (User.Identity.IsAuthenticated)
            {
                // FIX: Correctly redirect Admin to their main dashboard view.
                if (User.IsInRole("Admin"))
                {
                    return RedirectToAction("Index", "Admin");
                }

                // FIX: Correctly redirect WeddingPlanner to their main dashboard view.
                if (User.IsInRole("WeddingPlanner"))
                {
                    return RedirectToAction("Index", "Planner");
                }

                // Couple redirection is correct.
                if (User.IsInRole("Couple"))
                {
                    return RedirectToAction("Index", "Dashboard");
                }
            }

            // If not authenticated, show the public home page.
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}