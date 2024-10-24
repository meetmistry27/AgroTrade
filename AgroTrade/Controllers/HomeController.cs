using AgroTrade.Models;
using AgroTrade.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AgroTrade.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserService _userService;

        public HomeController(ILogger<HomeController> logger, UserService userService)
        {
            _userService = userService;
            _logger = logger;
        }

        public async Task<IActionResult> chat(int id)
        {
            var user = await _userService.GetUserProfile(id);
            if (user == null)
            {
                return RedirectToAction("Error");
            }
            return View(user); 
        }


        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
