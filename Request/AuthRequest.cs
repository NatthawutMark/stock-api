
using System;
namespace stock_api.Request;

public static class AuthRequest
{
    public class login
    {
        public string username { get; set; }
        public string password { get; set; }
    }

    public class RefreshRequestDto
    {
        public string RefreshToken { get; set; } = null!;
    }

    public class AuthResponseDto
    {
        public string AccessToken { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
    }
}
