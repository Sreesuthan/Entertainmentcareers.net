using Dapper;
using Entertainmentcareers.net.Server.Data;
using Entertainmentcareers.net.Shared;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using MimeKit.Text;
using Newtonsoft.Json.Linq;
using Stripe;
using Stripe.Checkout;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Entertainmentcareers.net.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationUserController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;

        public ApplicationUserController(DataContext context, IConfiguration configuration)
        {
            StripeConfiguration.ApiKey = "sk_test_51M6VuGSEVEH46znALqeCQfXrgUc0MGnhBdKS7ewLLvQCFszM7zlwm325krHsNNPI8QTxEXhohUzhC7K3Fsm76LLZ00sQ5cdMKf";
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("emailcheck")]
        public async Task<ActionResult<string>> EmailCheck(SignUp request)
        {
            if (_context.Members.Any(u => u.Email == request.Email))
            {
                return BadRequest("Member already exists.");
            }
            else
            {
                return Ok();
            }
        }

        [HttpPost("signup")]
        public async Task<ActionResult<string>> SignUp(SignUp request)
        {
            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);
            var token = CreateRandomToken();
            var member = new ApplicationUser
            {
                Email = request.Email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Street = request.Street,
                City = request.City,
                State = request.State,
                Zip = request.Zip,
                PlanType = request.PlanType,
                ConfirmationToken = token
            };

            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            String link = $"https://localhost:7119/ConfirmEmail/{token}";
            SendEmail(link, request.Email);

            _context.Members.Add(member);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("signin")]
        public async Task<ActionResult<string>> SignIn(SignIn request)
        {
            var user = await _context.Members.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
            {
                return BadRequest("Username is Incorrect!");
            }

            if (!VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
            {
                return BadRequest("Password is incorrect!");
            }

            if (user.IsConfirmed == false)
            {
                return BadRequest("Email not confirmed!");
            }
            string token = CreateToken(user);

            return Ok(token);
        }

        [HttpPost("resendverify")]
        public async Task<IActionResult> Verify(ForgotPassword email)
        {
            var user = await _context.Members.FirstOrDefaultAsync(u => u.Email == email.Email);
            if (user == null)
            {
                return BadRequest("Email not found.");
            }

            string token = user.ConfirmationToken;
            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            String link = $"https://localhost:7119/ConfirmEmail/{token}";
            SendEmail(link, email.Email);

            return Ok(); 
        }

        [HttpPost("verify")]
        public async Task<IActionResult> Verify(string token)
        {
            String _token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
            var user = await _context.Members.FirstOrDefaultAsync(u => u.ConfirmationToken == _token);
            if (user == null)
            {
                return BadRequest("Invalid token.");
            }

            user.ConfirmedAt = DateTime.Now;
            user.IsConfirmed = true;
            await _context.SaveChangesAsync();

            return Ok("User verified! :)");
        }

        [HttpPost("forgotpassword")]
        public async Task<IActionResult> ForgotPassword(ForgotPassword forgotpassword)
        {
            var user = await _context.Members.FirstOrDefaultAsync(u => u.Email == forgotpassword.Email);
            if (user == null)
            {
                return BadRequest("User not found.");
            }
            var token = CreateRandomToken();
            user.PasswordResetToken = token;
            user.ResetTokenExpires = DateTime.Now.AddDays(1);

            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            String link = $"https://localhost:7119/resetpassword/{token}";
            SendEmail1(link, forgotpassword.Email);

            await _context.SaveChangesAsync();

            return Ok("You may now reset your password.");
        }

        [HttpPost("resetpassword")]
        public async Task<IActionResult> ResettPassword(ResetPassword request)
        {
            String _token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Token));
            var user = await _context.Members.FirstOrDefaultAsync(u => u.PasswordResetToken == _token);
            if (user == null || user.ResetTokenExpires < DateTime.Now)
            {
                return BadRequest("Invalid Token.");
            }

            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            user.PasswordResetToken = null;
            user.ResetTokenExpires = null;

            await _context.SaveChangesAsync();

            return Ok("Password successfully reset.");
        }

        [HttpGet("user/{username}")]
        public async Task<ActionResult<ApplicationUser>> GetUser(string username)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            var user = await connection.QueryFirstAsync<ApplicationUser>("select * from Members where Email = @UserName", new { UserName = username });
            return Ok(user);
        }

        [HttpPut]
        public async Task<ActionResult<List<ApplicationUser>>> UpdateJob(ApplicationUser user)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("update Members set FirstName=@FirstName, LastName=@LastName, Street=@Street, City=@City, State=@State, Zip=@Zip, ResumePath=@ResumePath, ResumeFileContentType=@ResumeFileContentType, ResumeOriginalFileName=@ResumeOriginalFileName, ResumeStoredFileName=@ResumeStoredFileName, CoverOriginalFileName=@CoverOriginalFileName, CoverLetterPath=@CoverLetterPath, CoverFileContentType=@CoverFileContentType, CoverStoredFileName=@CoverStoredFileName where Email=@Email", user);  
            return Ok(await SelectAllUsers(connection));
        }

        [HttpPost("checkout")]
        public ActionResult CreateCheckoutSession(SignUp signUp)
        {
            var lineItems = new List<SessionLineItemOptions>
            {
                new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmountDecimal = signUp.PlanPrice * 100,
                        Currency = "inr",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = signUp.PlanType,
                        },
                    },
                    Quantity = 1,
                },
            };
            var options = new SessionCreateOptions
            {
                LineItems = lineItems,
                Mode = "payment",
                SuccessUrl = "https://localhost:7119/signin",
                CancelUrl = "https://localhost:7119/signup",
            };

            var service = new SessionService();
            Session session = service.Create(options);
            return Ok(session.Url);
        }

        private static async Task<IEnumerable<ApplicationUser>> SelectAllUsers(SqlConnection connection)
        {
            return await connection.QueryAsync<ApplicationUser>("select * from Members");
        }

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        private static bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }

        private string CreateToken(ApplicationUser user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Role, "Job Seeker")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        private static string CreateRandomToken()
        {
            return Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
        }

        void SendEmail(string? body, string Email)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_configuration.GetSection("EmailUsername").Value));
            email.To.Add(MailboxAddress.Parse(Email));
            email.Subject = "Confirm your Email";
            email.Body = new TextPart(TextFormat.Text) { Text = body };

            using var smtp = new SmtpClient();
            smtp.Connect(_configuration.GetSection("EmailHost").Value, 587, SecureSocketOptions.StartTls);
            smtp.Authenticate(_configuration.GetSection("EmailUsername").Value, _configuration.GetSection("EmailPassword").Value);
            smtp.Send(email);
            smtp.Disconnect(true);
        }

        void SendEmail1(string? body, string Email)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_configuration.GetSection("EmailUsername").Value));
            email.To.Add(MailboxAddress.Parse(Email));
            email.Subject = "Reset your password";
            email.Body = new TextPart(TextFormat.Text) { Text = body };

            using var smtp = new SmtpClient();
            smtp.Connect(_configuration.GetSection("EmailHost").Value, 587, SecureSocketOptions.StartTls);
            smtp.Authenticate(_configuration.GetSection("EmailUsername").Value, _configuration.GetSection("EmailPassword").Value);
            smtp.Send(email);
            smtp.Disconnect(true);
        }
    }
}
