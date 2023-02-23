using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entertainmentcareers.net.Shared
{
    public class ForgotPassword
    {
        [Required]
        [EmailAddress(ErrorMessage ="Enter valid email address.")]
        public string Email { get; set; } = string.Empty;
    }
}
