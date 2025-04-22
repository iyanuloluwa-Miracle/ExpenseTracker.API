// File: DTOs/AuthResponseDTOs.cs
namespace AuthSystem.DTOs
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