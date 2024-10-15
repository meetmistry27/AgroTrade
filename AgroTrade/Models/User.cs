using System;
using System.ComponentModel.DataAnnotations;

namespace AgroTrade.Models
{
    public class User
    {
        public int UserId { get; set; } 

        [Required(ErrorMessage = "Username is required.")]
        [StringLength(50, ErrorMessage = "Username cannot exceed 50 characters.")]
        public string Username { get; set; } 

        [StringLength(255, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string Password { get; set; } 

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } 

        [Phone(ErrorMessage = "Invalid phone number format.")]
        public string Phone { get; set; }

        public ICollection<Crop> Crops { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; 
    }
}
