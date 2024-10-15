using AgroTrade.Models;
using AgroTrade.Services;
using Microsoft.AspNetCore.Mvc;

namespace AgroTrade.Controllers
{
    public class UserController : Controller
    {
        private UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        public ViewResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(User user)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.RegisterUser(user);
                if (result)
                {
                    return RedirectToAction("Login");
                }
                ModelState.AddModelError("", "User already exists.");
            }
            return View(user);
        }


        public ViewResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            if (ModelState.IsValid)
            {
                var user = await _userService.AuthenticateUser(email, password);
                if (user != null)
                {
                    return RedirectToAction("Profile", new { id = user.UserId });
                }
                ModelState.AddModelError("", "Invalid login attempt.");
            }
            return View();
        }


        public async Task<IActionResult> Profile(int id)
        {
            var user = await _userService.GetUserProfile(id);
            if (user == null) return NotFound();
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(User user)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.UpdateUserProfile(user);
                if (result)
                {
                    return RedirectToAction("Profile", new { id = user.UserId });
                }
                ModelState.AddModelError("", "Profile update failed.");
            }
            return View(user);
        }
    }
}
