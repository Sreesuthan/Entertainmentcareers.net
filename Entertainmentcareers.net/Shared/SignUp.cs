using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entertainmentcareers.net.Shared
{
    public class SignUp
    {
        [Required]
        [EmailAddress(ErrorMessage ="Enter valid email address.")]
        public string Email { get; set; } = string.Empty;
        [Required, ]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[$@$!%*?&])[A-Za-z\d$@$!%*?&]{6,}", ErrorMessage = "Password must contains 6 characters, digit (0-9), uppercase(A-Z) and special character.")]
        public string Password { get; set; } = string.Empty;
        [Required]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        public string LastName { get; set; } = string.Empty;
        [Required]
        public string Street { get; set; } = string.Empty;
        [Required]
        public string City { get; set; } = string.Empty;
        [Required]
        public string State { get; set; } = string.Empty;
        [Required]
        public string Zip { get; set; } = string.Empty;
        [Required]
        public string PlanType { get; set; } = "Monthly";
        [Required]
        public string CreditCard { get; set; } = string.Empty;
        [Required]
        public string Month { get; set; } = string.Empty;
        [Required]
        public string Year { get; set; } = string.Empty;
        public string About { get; set; } = string.Empty;
        [Required]
        public bool CheckBox { get; set; } = true;
    }
}
