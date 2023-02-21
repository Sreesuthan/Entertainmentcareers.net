using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entertainmentcareers.net.Shared
{
    public class SignIn
    {
        [Required]
        [EmailAddress(ErrorMessage = "Enter valid email address.")]
        public string Email { get; set; } = string.Empty;
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
        public List<string> Role { get; set; } = new List<string>();
        public bool RememberMe { get; set; }

    }
}
