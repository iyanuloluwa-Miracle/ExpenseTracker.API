// File: Controllers/AuthController.cs
using Server.DTOs;
using Server.Models;
using Server.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.IdentityModel.Tokens.Jwt;

namespace Server.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IMongoDbService _mongoDb;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public AuthController(
            IMongoDbService mongoDb,
            IEmailService emailService,
            IConfiguration configuration)
        {
            _mongoDb = mongoDb;
            _emailService = emailService;
            _configuration = configuration;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromBody] SignupRequest request)
        {
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest("Email and password are required");
            }

            // Check if email already exists
            var existingUser = await _mongoDb.Users.Find(u => u.Email == request.Email).FirstOrDefaultAsync();
            if (existingUser != null)
            {
                return BadRequest("Email already registered");
            }

            // Create password hash and salt
            CreatePasswordHash(request.Password, out string passwordHash, out string passwordSalt);

            // Generate email verification token
            string verificationToken = GenerateRandomToken();

            // Create new user
            var user = new User
            {
                Email = request.Email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                IsEmailVerified = false,
                EmailVerificationToken = verificationToken,
                CreatedAt = DateTime.UtcNow,
            };

            await _mongoDb.Users.InsertOneAsync(user);

            // Send verification email
            await _emailService.SendVerificationEmailAsync(request.Email, verificationToken);

            return Ok(new MessageResponse { Message = "Registration successful. Please check your email to verify your account." });
        }

        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailRequest request)
        {
            var user = await _mongoDb.Users.Find(u => u.EmailVerificationToken == request.Token).FirstOrDefaultAsync();
            if (user == null)
            {
                return BadRequest("Invalid verification token");
            }

            var update = Builders<User>.Update
                .Set(u => u.IsEmailVerified, true)
                .Set(u => u.EmailVerificationToken, null);

            await _mongoDb.Users.UpdateOneAsync(u => u.Id == user.Id, update);

            return Ok(new MessageResponse { Message = "Email verified successfully. You can now log in." });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _mongoDb.Users.Find(u => u.Email == request.Email).FirstOrDefaultAsync();
            if (user == null)
            {
                return BadRequest("Invalid email or password");
            }

            if (!VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
            {
                return BadRequest("Invalid email or password");
            }

            if (!user.IsEmailVerified)
            {
                return BadRequest("Please verify your email before logging in");
            }

            // Update last login time
            var update = Builders<User>.Update.Set(u => u.LastLogin, DateTime.UtcNow);
            await _mongoDb.Users.UpdateOneAsync(u => u.Id == user.Id, update);

            // Generate JWT token
            string token = GenerateJwtToken(user);

            return Ok(new LoginResponse { 
                Token = token, 
            });
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            var user = await _mongoDb.Users.Find(u => u.Email == request.Email).FirstOrDefaultAsync();
            if (user == null)
            {
                // Don't reveal if email exists or not for security reasons
                return Ok(new MessageResponse { Message = "If your email is registered, you will receive a password reset link" });
            }

            // Create a reset token
            string resetToken = GenerateRandomToken();
            
            // Save token to database
            var token = new Token
            {
                UserId = user.Id,
                TokenValue = resetToken,
                Type = "reset",
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                IsUsed = false
            };

            await _mongoDb.Tokens.InsertOneAsync(token);

            // Send reset email
            await _emailService.SendPasswordResetEmailAsync(request.Email, resetToken);

            return Ok(new MessageResponse { Message = "If your email is registered, you will receive a password reset link" });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            if (string.IsNullOrEmpty(request.Token) || string.IsNullOrEmpty(request.NewPassword))
            {
                return BadRequest("Token and new password are required");
            }

            // Find the token
            var token = await _mongoDb.Tokens.Find(t => 
                t.TokenValue == request.Token && 
                t.Type == "reset" && 
                t.ExpiresAt > DateTime.UtcNow && 
                !t.IsUsed).FirstOrDefaultAsync();

            if (token == null)
            {
                return BadRequest("Invalid or expired token");
            }

            // Find the user
            var user = await _mongoDb.Users.Find(u => u.Id == token.UserId).FirstOrDefaultAsync();
            if (user == null)
            {
                return BadRequest("User not found");
            }

            // Update the password
            CreatePasswordHash(request.NewPassword, out string passwordHash, out string passwordSalt);
            var update = Builders<User>.Update
                .Set(u => u.PasswordHash, passwordHash)
                .Set(u => u.PasswordSalt, passwordSalt);

            await _mongoDb.Users.UpdateOneAsync(u => u.Id == user.Id, update);

            // Mark token as used
            var tokenUpdate = Builders<Token>.Update.Set(t => t.IsUsed, true);
            await _mongoDb.Tokens.UpdateOneAsync(t => t.Id == token.Id, tokenUpdate);

            return Ok(new MessageResponse { Message = "Password reset successful. You can now log in with your new password." });
        }

        private string GenerateJwtToken(User user)
{
    var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id),
        new Claim(ClaimTypes.Email, user.Email)
    };

    var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY") ?? 
        throw new InvalidOperationException("JWT_KEY is not set");
    
    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    var expires = DateTime.Now.AddDays(1);

    var token = new JwtSecurityToken(
        issuer: Environment.GetEnvironmentVariable("JWT_ISSUER"),
        audience: Environment.GetEnvironmentVariable("JWT_AUDIENCE"),
        claims: claims,
        expires: expires,
        signingCredentials: creds
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
}

        private void CreatePasswordHash(string password, out string passwordHash, out string passwordSalt)
        {
            using var hmac = new HMACSHA512();
            var salt = Convert.ToBase64String(hmac.Key);
            var hash = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(password)));
            
            passwordSalt = salt;
            passwordHash = hash;
        }

        private bool VerifyPasswordHash(string password, string storedHash, string storedSalt)
        {
            var saltBytes = Convert.FromBase64String(storedSalt);
            using var hmac = new HMACSHA512(saltBytes);
            var computedHash = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(password)));
            return computedHash == storedHash;
        }

        private string GenerateRandomToken()
        {
            using var rng = RandomNumberGenerator.Create();
            var tokenBytes = new byte[32];
            rng.GetBytes(tokenBytes);
            return Convert.ToBase64String(tokenBytes)
                .Replace("+", "-")
                .Replace("/", "_")
                .Replace("=", "");
        }
    }
}