using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entertainmentcareers.net.Shared
{
    public class ApplicationUser
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public byte[] PasswordHash { get; set; } = new byte[32];
        public byte[] PasswordSalt { get; set; } = new byte[32];
        public string? ConfirmationToken { get; set; }
        public DateTime? ConfirmedAt { get; set; }
        public bool IsConfirmed { get; set; }
        public string? PasswordResetToken { get; set; }
        public DateTime? ResetTokenExpires { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string Zip { get; set; } = string.Empty;
        public string PlanType { get; set; } = string.Empty;
        public string ResumePath { get; set; } = string.Empty;
        public string ResumeOriginalFileName { get; set; } = string.Empty;
        public string ResumeStoredFileName { get; set; } = string.Empty;
        public string ResumeFileContentType { get; set; } = string.Empty;
        public string CoverLetterPath { get; set; } = string.Empty;
        public string CoverOriginalFileName { get; set; } = string.Empty;
        public string CoverStoredFileName { get; set; } = string.Empty;
        public string CoverFileContentType { get; set; } = string.Empty;
    }
}
