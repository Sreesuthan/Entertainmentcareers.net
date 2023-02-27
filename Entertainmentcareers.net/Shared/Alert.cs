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
        public int Id { get; set; }
        [Required]
        [EmailAddress(ErrorMessage ="Enter valid email address.")]
        public string Email { get; set; } = string.Empty;
        [NotMapped]
        [Required]
        [Compare(nameof(Email), ErrorMessage = "Email and Confirm Email Should be same")]
        public string ConfirmEmail { get; set; } = string.Empty;
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Zip { get; set; } = string.Empty;
        public bool NewsLetterJob { get; set; } = false;
        public bool NewsLetterIntership { get; set; } = false;
        public string ConfirmationToken { get; set; } = string.Empty;
        public DateTime? ConfirmedAt { get; set; } = null;
        public bool IsConfirmed { get; set; } = false;
    }
}
