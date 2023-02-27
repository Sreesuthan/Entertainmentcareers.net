using Dapper;
using Entertainmentcareers.net.Shared;
using MailKit.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Data.SqlClient;
using MimeKit.Text;
using MimeKit;
using System.Security.Cryptography;
using System.Text;
using static Org.BouncyCastle.Math.EC.ECCurve;
using MailKit.Net.Smtp;
using Microsoft.EntityFrameworkCore;
using static MudBlazor.CategoryTypes;
using Entertainmentcareers.net.Server.Data;

namespace Entertainmentcareers.net.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlertController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _env;
        private readonly DataContext _context;

        public AlertController(IConfiguration config, IWebHostEnvironment env, DataContext context)
        {
            _config = config;
            _env = env;
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<List<Entertainmentcareers.net.Shared.Alert>>> CreateSubscriber(Entertainmentcareers.net.Shared.Alert alert)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            if (_context.AlertSubscribers.Any(u => u.Email == alert.Email))
            {
                return BadRequest("Member already exists.");
            }
            var token = CreateRandomToken();
            alert.ConfirmationToken = token;
            await connection.ExecuteAsync("Insert into AlertSubscribers values(@Email, @Name, @Zip, @NewsLetterJob, @NewsLetterIntership, @ConfirmationToken, @ConfirmedAt, @IsConfirmed)", alert);

            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var email = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(alert.Email));
            string link = $"https://localhost:7119/ConfirmAlertEmail/{token}/{email}";
            SendEmail(link, alert.Email);
            return Ok();
        }

        [HttpPost("verify")]
        public async Task<IActionResult> Verify(Entertainmentcareers.net.Shared.Token token)
        {
            string _code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token.VerificationToken));
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            var subscriber = await connection.QueryFirstAsync<Entertainmentcareers.net.Shared.Alert>("select * from AlertSubscribers where ConfirmationToken = @ConfirmationToken", new { ConfirmationToken = _code });
            if (subscriber == null)
            {
                return BadRequest("Invalid token.");
            }

            subscriber.ConfirmedAt = DateTime.Now;
            subscriber.IsConfirmed = true;
            await connection.ExecuteAsync("update AlertSubscribers set ConfirmedAt=@ConfirmedAt, IsConfirmed=@IsConfirmed", subscriber);
            
            return Ok("User verified! :)");
        }

        [HttpGet("{email}")]
        public async Task<ActionResult<Entertainmentcareers.net.Shared.Alert>> GetSubscriber(string email)
        {
            try
            {
                using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                var subscriber = await connection.QueryFirstAsync<Entertainmentcareers.net.Shared.Alert>("select * from AlertSubscribers where Email=@Email",
                    new { Email = email });
                return Ok(subscriber);
            }
            catch
            {
                return BadRequest("subscriber details not found...");
            }
        }

        private static string CreateRandomToken()
        {
            return Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
        }

        private void SendEmail(string body, string Email)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_config.GetSection("EmailUsername").Value));
            email.To.Add(MailboxAddress.Parse(Email));
            email.Subject = "Confirm your Email";
            email.Body = new TextPart(TextFormat.Html) { Text = CreateBody(body) };

            using var smtp = new SmtpClient();
            smtp.Connect(_config.GetSection("EmailHost").Value, 587, SecureSocketOptions.StartTls);
            smtp.Authenticate(_config.GetSection("EmailUsername").Value, _config.GetSection("EmailPassword").Value);
            smtp.Send(email);
            smtp.Disconnect(true);
        }

        private string CreateBody(string body)
        {
            var newBody = System.IO.File.ReadAllText(Path.Combine(_env.ContentRootPath, "EmailBody", "JobAlert.html"));
            newBody = newBody.Replace("{link}", body);
            return newBody;
        }
    }
}
