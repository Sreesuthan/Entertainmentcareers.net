using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entertainmentcareers.net.Shared
{
    public class Alert
    {
        [Required]
        [EmailAddress(ErrorMessage ="Enter valid email address.")]
        public string Email { get; set; } = string.Empty;
        [Required]
        [Compare(nameof(Email), ErrorMessage = "Email and Confirm Email Should be same")]
        public string ConfirmEmail { get; set; } = string.Empty;
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Zip { get; set; } = string.Empty;
        public bool NewsLetterJob { get; set; }
        public bool NewsLetterIntership { get; set; }
    }
}
