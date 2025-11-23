using DreamDay.Data;
using DreamDay.Models;
using DreamDay.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DreamDay.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ApplicationDbContext _context;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (User.Identity.IsAuthenticated) return RedirectToAction("Index", "Home");
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                var passwordCheck = await _userManager.CheckPasswordAsync(user, model.Password);
                if (passwordCheck)
                {
                    var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);
                    if (result.Succeeded)
                    {
                        // --- THIS IS THE FIX ---
                        // Instead of trying to figure out the role here,
                        // we redirect to the central HomeController, which already has the correct logic.
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            TempData["Error"] = "Wrong credentials. Please, try again!";
            return View(model);
        }

        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View(new RegisterViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (User.Identity.IsAuthenticated) return RedirectToAction("Index", "Home");

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                TempData["Error"] = "This email address is already in use";
                return View(model);
            }

            var newUser = new User() { Name = model.Name, Email = model.Email, UserName = model.Email };
            var result = await _userManager.CreateAsync(newUser, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(newUser, model.Role);

                if (model.Role == "Couple")
                {
                    _context.Couples.Add(new Couple { UserId = newUser.Id });
                }
                else if (model.Role == "WeddingPlanner")
                {
                    _context.WeddingPlanners.Add(new WeddingPlanner { UserId = newUser.Id });
                }
                await _context.SaveChangesAsync();

                return RedirectToAction("Login");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}