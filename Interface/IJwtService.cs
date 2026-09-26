namespace stock_api.Interfaces;

public interface IJwtService
{
    string GenerateAccessToken(string userId, string username, string role);
    string GenerateRefreshToken();
}