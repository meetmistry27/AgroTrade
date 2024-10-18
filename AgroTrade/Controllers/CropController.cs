using Microsoft.AspNetCore.Mvc;
using AgroTrade.Models;
using AgroTrade.Services;
using System.IO;
using System.Threading.Tasks;

namespace AgroTrade.Controllers
{
    public class CropController : Controller
    {
        private readonly CropService _cropService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserService _userService;

        public CropController(CropService cropService, UserService userService, IWebHostEnvironment webHostEnvironment)
        {
            _cropService = cropService;
            _userService = userService;
            _webHostEnvironment = webHostEnvironment;
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

                if (crop.Image != null && crop.Image.Length > 0)
                {
                    var fileName = Path.GetFileNameWithoutExtension(crop.Image.FileName);
                    var extension = Path.GetExtension(crop.Image.FileName);
                    fileName = fileName + "_" + Path.GetRandomFileName() + extension;

                    var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await crop.Image.CopyToAsync(stream);
                    }

                    crop.ImagePath = "/uploads/" + fileName;
                }

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

                var existingCrop = await _cropService.GetcropByid(crop.CropId);
                if (existingCrop == null)
                {
                    return NotFound();  
                }

                existingCrop.CropName = crop.CropName;
                existingCrop.Quantity = crop.Quantity;
                existingCrop.Price = crop.Price;

                if (crop.Image != null && crop.Image.Length > 0)
                {
                    var fileName = Path.GetFileNameWithoutExtension(crop.Image.FileName);
                    var extension = Path.GetExtension(crop.Image.FileName);
                    fileName = fileName + "_" + Path.GetRandomFileName() + extension;

                    var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await crop.Image.CopyToAsync(stream);
                    }

                    existingCrop.ImagePath = "/uploads/" + fileName;
                }
                else
                {
                    existingCrop.ImagePath = existingCrop.ImagePath; 
                }
                
                await _cropService.UpdateCropAsync(existingCrop);
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
