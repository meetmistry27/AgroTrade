using System;
using System.ComponentModel.DataAnnotations;

namespace AgroTrade.Models
{
    public class Crop
    {
        public int CropId { get; set; } 

        [Required(ErrorMessage = "Crop name is required.")]
        [StringLength(50, ErrorMessage = "Crop name cannot exceed 50 characters.")]
        public string CropName { get; set; } 

        [Required(ErrorMessage = "Quantity is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0.")]
        public int Quantity { get; set; } 

        [Required(ErrorMessage = "Price is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        public decimal Price { get; set; } 

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required(ErrorMessage = "User ID is required.")]
        public int UserId { get; set; }
        public User? User { get; set; } 
    }
}
