using System;
using System.ComponentModel.DataAnnotations;

namespace AgroTrade.Models
{
    public class Transaction
    {
        public int TransactionId { get; set; }

        [Required(ErrorMessage = "Quantity is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0.")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Total price is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Total price must be greater than 0.")]
        public decimal TotalPrice { get; set; }

        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

        public int BuyerId { get; set; }
        public User Buyer { get; set; }

        public int SellerId { get; set; }
        public User Seller { get; set; }

        public int CropId { get; set; }
        public Crop Crop { get; set; }

        public TransactionStatus Status { get; set; } = TransactionStatus.Pending; 
    }

    public enum TransactionStatus
    {
        Pending,
        Accepted,
        Rejected
    }
}
