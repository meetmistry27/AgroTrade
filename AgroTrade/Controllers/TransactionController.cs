using Microsoft.AspNetCore.Mvc;
using AgroTrade.Models;
using AgroTrade.Models.ViewModel;
using AgroTrade.Services;
using System.Threading.Tasks;
using System.Linq;

namespace AgroTrade.Controllers
{
    [Route("transactions")]
    public class TransactionController : Controller
    {
        private readonly CropService _cropService;
        private readonly TransactionService _transactionService;

        public TransactionController(CropService cropService, TransactionService transactionService)
        {
            _cropService = cropService;
            _transactionService = transactionService;
        }

        [HttpGet("index")]
        public async Task<IActionResult> Index()
        {
            int userId = GetUserId();
            var transactions = await _transactionService.GetTransactionsByUserIdAsync(userId);
            return View(transactions);
        }

        [HttpGet("buy/{cropId}")]
        public async Task<IActionResult> Buy(int cropId)
        {
            var crop = await _cropService.GetcropByid(cropId);
            if (crop == null)
            {
                return NotFound();
            }

            var model = new TransactionViewModel
            {
                CropId = cropId,
                TotalPrice = crop.Price
            };

            return View(model);
        }

        [HttpPost("buy")]
        public async Task<IActionResult> CreateTransaction(TransactionViewModel transactionViewModel) // Changed the action name
        {
            if (ModelState.IsValid)
            {
                var crop = await _cropService.GetcropByid(transactionViewModel.CropId);
                if (crop == null)
                {
                    return NotFound();
                }

                if (transactionViewModel.Quantity > crop.Quantity)
                {
                    TempData["ErrorMessage"] = "Applying more quantity which is not available";
                    return View(transactionViewModel);
                }

                var transaction = new Transaction
                {
                    CropId = transactionViewModel.CropId,
                    BuyerId = GetUserId(),
                    Quantity = transactionViewModel.Quantity,
                    SellerId = crop.UserId,
                    TotalPrice = transactionViewModel.Quantity * crop.Price,
                    Status = TransactionStatus.Pending
                };

                await _transactionService.CreateTransactionAsync(transaction);
                return RedirectToAction("Index", "Crop");
            }

            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            TempData["ErrorMessage"] = string.Join(", ", errors);
            return View(transactionViewModel);
        }

        public async Task<IActionResult> Accept(int transactionId)
        {
            await _transactionService.AcceptTransactionAsync(transactionId);
            return RedirectToAction("index");
        }

        [HttpPost("reject")]
        public async Task<IActionResult> Reject(int transactionId)
        {
            await _transactionService.RejectTransactionAsync(transactionId);
            return RedirectToAction("index");
        }


        private int GetUserId()
        {
            if (Request.Cookies.TryGetValue("UserId", out var userIdString) && int.TryParse(userIdString, out var userId))
            {
                return userId;
            }

            throw new InvalidOperationException("UserId not found in cookies.");
        }
    }
}
