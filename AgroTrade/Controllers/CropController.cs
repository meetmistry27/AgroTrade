using Microsoft.AspNetCore.Mvc;
using AgroTrade.Models;
using AgroTrade.Services;

namespace AgroTrade.Controllers
{
    public class CropController : Controller
    {
        private readonly CropService _cropService;
        private readonly UserService _userService;
        public CropController(CropService cropService,UserService userService)
        {
            _userService = userService;
            _cropService = cropService;
        }

        private int GetUserId()
        {
            if (Request.Cookies.TryGetValue("UserId", out var userIdString) && int.TryParse(userIdString, out var userId))
            {
                return userId; 
            }

            throw new InvalidOperationException("UserId not found in cookies.");
        }


        public async Task<IActionResult> Index()
        {
            var crops = await _cropService.GetAllCropsAsync();
            return View(crops);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Crop crop)
        {
            if (ModelState.IsValid)
            {
                crop.UserId = GetUserId();
                var user = await _userService.GetUserProfile(crop.UserId);
                crop.User = user;
                await _cropService.CreateCropAsync(crop);

                return RedirectToAction(nameof(Index));
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                TempData["ErrorMessage"] = string.Join(", ", errors);  
            }
            return View(crop);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var crop = await _cropService.GetcropByid(id);
            if (crop == null)
            {
                return NotFound();
            }
            return View(crop);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Crop crop)
        {
            if (ModelState.IsValid)
            {
                crop.UserId = GetUserId();
                await _cropService.UpdateCropAsync(crop);
                return RedirectToAction(nameof(Index));
            }
            return View(crop);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var crop = await _cropService.GetcropByid(id);
            if (crop == null)
            {
                return NotFound();
            }
            return View(crop); 
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int CropId) 
        {
            await _cropService.DeleteCropAsync(CropId); 
            return RedirectToAction(nameof(Index)); 
        }
    }
}
