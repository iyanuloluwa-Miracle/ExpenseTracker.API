// File: Services/EmailService.cs
using Server.Configuration;
using Server.Services.Interfaces;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace Server.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = false)
        {
            var message = new MailMessage
            {
                From = new MailAddress(_emailSettings.FromAddress, _emailSettings.FromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = isHtml
            };
            message.To.Add(to);

            using var client = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort)
            {
                Credentials = new NetworkCredential(_emailSettings.SmtpUsername, _emailSettings.SmtpPassword),
                EnableSsl = true
            };

            await client.SendMailAsync(message);
        }

        public async Task SendVerificationEmailAsync(string email, string token)
        {
            string subject = "Verify Your Email Address";
            string verificationUrl = $"http://localhost:5130/Accounts/Register/verify-email?token={token}";
            string body = $@"
                <html>
                <body>
                    <h2>Email Verification</h2>
                    <p>Thank you for registering! Please verify your email by clicking the link below:</p>
                    <p><a href='{verificationUrl}'>Verify Email</a></p>
                    <p>If you didn't create an account, you can ignore this email.</p>
                </body>
                </html>";

            await SendEmailAsync(email, subject, body, true);
        }

        public async Task SendPasswordResetEmailAsync(string email, string token)
        {
            string subject = "Reset Your Password";
            string resetUrl = $"http://localhost:5130/Accounts/ForgotPassword?token={token}";
            string body = $@"
                <html>
                <body>
                    <h2>Password Reset</h2>
                    <p>You requested a password reset. Click the link below to reset your password:</p>
                    <p><a href='{resetUrl}'>Reset Password</a></p>
                    <p>If you didn't request a password reset, please ignore this email.</p>
                    <p>This link will expire in 1 hour.</p>
                </body>
                </html>";

            await SendEmailAsync(email, subject, body, true);
        }
    }
}