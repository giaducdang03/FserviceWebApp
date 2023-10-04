﻿using System.ComponentModel.DataAnnotations;

namespace FServiceAPI.Models
{
    public class SignUpModel
    {
        [Required]
        public string? Name { get; set; }
        [Required]
        public string? PhoneNumber { get; set; }
        [Required]
        public string? Address { get; set; }
        [Required]
        public string? DateOfBirth { get; set; }
        [Required]
        public string Role { get; set; }
        [Required, EmailAddress]
        public string Email { get; set; } = null!;
        [Required, EmailAddress]
        public string UserName { get; set; } = null!;
        [Required]
        public string Password { get; set; } = null!;
        [Required]
        public string ConfirmPassword { get; set; } = null!;
    }
}
