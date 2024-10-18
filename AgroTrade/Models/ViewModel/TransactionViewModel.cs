using System.ComponentModel.DataAnnotations;

namespace AgroTrade.Models.ViewModel
{
    public class TransactionViewModel
    {
        public int CropId { get; set; }

        [Required(ErrorMessage = "Buyer field is required.")]
        public int BuyerId { get; set; }

        [Required(ErrorMessage = "Quantity is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0.")]
        public int Quantity { get; set; }

        public decimal TotalPrice { get; set; }
    }
}
