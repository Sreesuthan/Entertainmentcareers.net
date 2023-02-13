using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entertainmentcareers.net.Shared
{
    public class Job
    {
        public int Id { get; set; }
        [Required]
        public string Email { get; set; } = string.Empty;
        public string EmploymentType { get; set; } = string.Empty;
        [Required]
        public string JobType { get; set; } = string.Empty;
        [Required]
        [MaxLength(50, ErrorMessage = "Max Length is {1} Characters.")]
        public string CompanyName { get; set; } = string.Empty;
        [Required]
        public bool ShowCompanyName { get; set; }
        [Required]
        [MaxLength(50, ErrorMessage = "Max Length is {1} Characters.")]
        public string JobTitle { get; set; } = string.Empty;
        public string JobCode { get; set; } = string.Empty;
        [Required]
        public string City { get; set; } = string.Empty;
        [Required]
        public string Country { get; set; } = string.Empty;
        [Required]
        public string State { get; set; } = string.Empty;
        [Required]
        public string Website { get; set; } = string.Empty;
        [Required]
        public string Details { get; set; } = string.Empty;
        [Required]
        public string Instructions { get; set; } = string.Empty;
        [Required]
        public string Salary { get; set; } = string.Empty;
        public string AdditionalComments { get; set; } = string.Empty;
        public string InstructionForUs { get; set; } = string.Empty;
        public bool Active { get; set; }
        public string Employer { get; set; } = string.Empty;
        [Required]
        public string Category { get; set; } = string.Empty;
        public DateTime? CreateDate { get; set; }
        [Required]
        public DateTime? LastDateToApply { get; set; }
        [NotMapped]
        public long DateDiffMin { get; set; }
    }
}
