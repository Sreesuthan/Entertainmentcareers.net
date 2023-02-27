using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entertainmentcareers.net.Shared
{
    public class ResetPassword
    {
        [Required]
        [EmailAddress(ErrorMessage ="Enter valid email address.")]
        public string Email { get; set; } = string.Empty; 
        [Required]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[$@$!%*?&])[A-Za-z\d$@$!%*?&]{6,}", ErrorMessage = "Password must contains 6 characters, digit (0-9), uppercase(A-Z) and special character.")]
        public string Password { get; set; } = string.Empty;
        [Required]
        [Compare(nameof(Password), ErrorMessage = "Password and Confirm Password Should be same")]
        public string ConfirmPassword { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }
}
