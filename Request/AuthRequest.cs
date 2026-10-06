
using System;
namespace stock_api.Request;

public static class AuthRequest
{
    public class loginRequest
    {
        public string username { get; set; }
        public string password { get; set; }
    }

    public class RefreshtokenRequest
    {
        public string Id { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public string Token { get; set; } = null!;
        public DateTime ExpiryDate { get; set; }
        public bool IsRevoked { get; set; }
        public DateTime CreateDate { get; set; }
    }

    public class RefreshRequest
    {
        public string RefreshToken { get; set; } = null!;
    }

    public class AuthResponseDto
    {
        public string AccessToken { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
    }
}
