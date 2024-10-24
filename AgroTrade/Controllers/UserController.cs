using AgroTrade.Models;
using AgroTrade.Services;
using Microsoft.AspNetCore.Authorization;
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
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                TempData["ErrorMessage"] = string.Join(", ", errors);
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
                    var cookieOptions = new CookieOptions
                    {
                        HttpOnly = true,
                        Expires = DateTimeOffset.UtcNow.AddHours(1) 
                    };

                    HttpContext.Response.Cookies.Append("UserId", user.UserId.ToString(), cookieOptions);

                    return RedirectToAction("Profile", new { id = user.UserId });
                }
                TempData["failure"] = "password is incorrect!";
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Profile(int id)
        {
            var user = await _userService.GetUserProfile(id);
            if (user == null) return NotFound();
            return View(user);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _userService.GetUserProfile(id);
            if (user == null) return NotFound();
            return View(user);
        }


        [HttpPost]
        public async Task<IActionResult> Edit(User user)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.UpdateUserProfile(user);
                if (result)
                {
                    TempData["SuccessMessage"] = "Profile updated successfully!";
                    return RedirectToAction("Profile", new { id = user.UserId });
                }
                ModelState.AddModelError("", "Profile update failed."); 
            }

            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            TempData["ErrorMessage"] = string.Join(", ", errors);
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Response.Cookies.Delete("UserId");

            return RedirectToAction("Login", "User");
        }

    }
}
