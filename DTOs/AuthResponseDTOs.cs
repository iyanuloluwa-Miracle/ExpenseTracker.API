// File: DTOs/AuthResponseDTOs.cs
namespace Server.DTOs
{
    public class MessageResponse
    {
        public string Message { get; set; } = string.Empty;
    }

    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
    
    }
}