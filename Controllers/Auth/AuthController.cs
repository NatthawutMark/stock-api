using Microsoft.AspNetCore.Mvc;
using stock_api.Interfaces;
using stock_api.Repositories.Dapper;
using static stock_api.Request.AuthRequest;
using System.Dynamic;
using static stock_api.Response.AuthRes;


namespace stock_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    public bool _status = true;
    public string _message = string.Empty;
    public string _error = string.Empty;
    private readonly UnitOfWork _unitOfWork;
    private readonly IJwtService _jwtService;
    private readonly ISystemService _systemService;
    private readonly IConfiguration _config;


    public AuthController(UnitOfWork unitOfWork, ISystemService systemService, IJwtService jwtService, IConfiguration config)
    {
        _unitOfWork = unitOfWork;
        _systemService = systemService;
        _jwtService = jwtService;
        _config = config;
    }

    [HttpPost("login", Name = "Login")]
    public async Task<ActionResult> Login([FromBody] loginRequest req)
    {
        try
        {
            loginResponse response = new loginResponse();
            RefreshtokenRequest refreshToken = new RefreshtokenRequest();
            dynamic reqLogin = new ExpandoObject();
            string AccessToken = string.Empty;
            string RefreshToken = string.Empty;
            if (req == null)
            {
                return StatusCode(200, new { message = "Invalid login data" });
            }

            var resUser = await _unitOfWork.Auths.LoginAsync(req);

            if (resUser == null)
            {
                return StatusCode(200, new { status = false, message = "ไม่พบผู้ใช้งาน กรุณาตรวจสอบข้อมูลให้ถูกต้อง" });
            }
            else
            {
                if (!resUser.IsActive)
                {
                    return StatusCode(200, new { status = false, message = "บัญชีผู้ใช้งานถูกระงับ กรุณาติดต่อผู้ดูแลระบบ" });
                }

                string userId = resUser.userid;

                var resRole = await _unitOfWork.Auths.GetListRole(userId);
                var userMenus = await _systemService.GetMenuByUser(resUser);

                response = new loginResponse
                {
                    username = resUser.username,
                    fName = resUser.fName,
                    lName = resUser.lName,
                    role = resRole,
                    Menus = userMenus
                };

                #region Generate JWT Token
                var expirationMinutes = double.Parse(_config["JwtSettings:RefreshTokenExpirationDays"] ?? "3");
                var accessToken = _jwtService.GenerateAccessToken(userId, resUser.username, resRole?.roleEn ?? string.Empty);
                var refreshTokenValue = _jwtService.GenerateRefreshToken();

                refreshToken = new RefreshtokenRequest
                {
                    Id = _systemService.GenGUID(),
                    UserId = userId,
                    Token = refreshTokenValue,
                    ExpiryDate = DateTime.Now.AddDays(expirationMinutes),
                    IsRevoked = false,
                    CreateDate = DateTime.Now
                };
                await _unitOfWork.Auths.AddRefreshTokenAsync(refreshToken);
                await _unitOfWork.CompleteAsync(); // Commit transaction

                AccessToken = accessToken;
                RefreshToken = refreshTokenValue;
                #endregion
            }

            return StatusCode(200, new { success = true, results = new { response, AccessToken, RefreshToken }, message = "Login successful" });
        }
        catch (Exception ex)
        {
            _unitOfWork.Dispose();
            return StatusCode(200, new { status = false, message = ex.Message, error = ex.InnerException?.Message });
        }
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequest request)
    {
        try
        {
            var storedToken = await _unitOfWork.Auths.GetRefreshTokenAsync(request.RefreshToken);

            if (storedToken == null || string.IsNullOrEmpty(storedToken.UserId))
            {
                return Unauthorized(new { message = "Token ไม่ถูกต้อง" });
            }

            // [REUSE DETECTION] ถ้า Token นี้ถูกยกเลิกไปแล้ว แต่ยังมีคนนำกลับมาใช้ แสดงว่าโดนแฮก!
            if (storedToken.IsRevoked)
            {
                await _unitOfWork.Auths.RevokeAllRefreshTokensByUserAsync(storedToken.UserId);
                await _unitOfWork.CompleteAsync(); // Commit transaction
                return Unauthorized(new { message = "พบความเสี่ยงด้านความปลอดภัย กรุณาเข้าสู่ระบบใหม่" });
            }

            // ตรวจสอบวันหมดอายุ
            if (storedToken.ExpiryDate < DateTime.Now)
            {
                return Unauthorized(new { message = "Refresh Token หมดอายุแล้ว" });
            }

            var resRole = await _unitOfWork.Auths.GetListRole(storedToken.UserId);
            var newRefreshTokenValue = _jwtService.GenerateRefreshToken();

            // [ROTATION PROCESS] ยกเลิก Token ใบเก่า
            await _unitOfWork.Auths.RevokeRefreshTokenAsync(request.RefreshToken, newRefreshTokenValue);

            // ออก Token ใบใหม่
            var newAccessToken = _jwtService.GenerateAccessToken(storedToken.UserId, "admin", resRole?.roleEn ?? string.Empty);

            var expirationDate = double.Parse(_config["JwtSettings:RefreshTokenExpirationDays"] ?? "3");

            var newRefreshToken = new RefreshtokenRequest
            {
                Id = _systemService.GenGUID(),
                UserId = storedToken.UserId,
                Token = newRefreshTokenValue,
                ExpiryDate = DateTime.Now.AddDays(expirationDate),
                IsRevoked = false,
                CreateDate = DateTime.Now
            };

            await _unitOfWork.Auths.AddRefreshTokenAsync(newRefreshToken);
            await _unitOfWork.CompleteAsync(); // Commit ทั้งการยกเลิกใบเก่าและเพิ่มใบใหม่พร้อมกัน

            return Ok(new AuthResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshTokenValue
            });
        }
        catch (Exception ex)
        {
            _unitOfWork.Dispose();
            return StatusCode(500, new { message = "เกิดข้อผิดพลาดในการรีเฟรชโทเค็น", error = ex.Message });
        }
    }
}
