using Microsoft.AspNetCore.Mvc;
using stock_api.Interfaces;
using stock_api.Repositories.Dapper;
using static stock_api.Request.AuthRequest;
using System.Dynamic;
using static stock_api.Response.AuthRes;
using stock_api.Models;
using Microsoft.EntityFrameworkCore;

namespace stock_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    public bool _status = true;
    public string _message = string.Empty;
    public string _error = string.Empty;
    private readonly DapperUnitOfWork _dpUnitOfWork;
    private readonly IJwtService _jwtService;
    private readonly ISystemService _systemService;
    private readonly DbContexts _dbContexts;
    private readonly IConfiguration _config;


    public AuthController(DbContexts dbContexts, DapperUnitOfWork dpUnitOfWork, ISystemService systemService, IJwtService jwtService, IConfiguration config)
    {
        _dbContexts = dbContexts;
        _dpUnitOfWork = dpUnitOfWork;
        _systemService = systemService;
        _jwtService = jwtService;
        _config = config;
    }

    [HttpPost("login", Name = "Login")]
    public async Task<ActionResult> Login([FromBody] login req)
    {
        try
        {
            loginResponse response = new loginResponse();
            Refreshtoken refreshToken = new Refreshtoken();
            dynamic reqLogin = new ExpandoObject();
            string AccessToken = string.Empty;
            string RefreshToken = string.Empty;
            if (req == null)
            {
                return StatusCode(200, new { message = "Invalid login data" });
            }

            reqLogin.username = req.username;
            reqLogin.password = req.password;

            var resUser = await _dpUnitOfWork.Auths.LoginAsync(reqLogin);

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

                var resRole = await _dpUnitOfWork.Auths.GetListRole(userId);
                #region Set Menus
                var resMenus = await _dpUnitOfWork.Auths.GetMenuByUserId(userId);
                var menuLookup = resMenus.ToLookup(m => m.parentID);

                List<Menus>? BuildMenuTree(string? currentParentId)
                {
                    var children = menuLookup[currentParentId].ToList();

                    if (!children.Any())
                        return null;

                    foreach (var child in children)
                    {
                        // นำ subMenus มาต่อให้กับเมนูย่อยระดับลึกลงไป
                        child.subMenus = BuildMenuTree(child.menuID);
                    }

                    return children.OrderBy(m => m.orderNo).ToList();
                }

                var rootMenus = resMenus.Where(m => string.IsNullOrEmpty(m.parentID)).OrderBy(m => m.orderNo).ToList();
                foreach (var root in rootMenus)
                {
                    root.subMenus = BuildMenuTree(root.menuID);
                }
                #endregion

                response = new loginResponse
                {
                    username = resUser.username,
                    fName = resUser.fName,
                    lName = resUser.lName,
                    role = resRole,
                    Menus = rootMenus.Any() ? rootMenus : null
                };

                #region Generate JWT Token
                var expirationMinutes = double.Parse(_config["JwtSettings:RefreshTokenExpirationDays"] ?? "3");
                var accessToken = _jwtService.GenerateAccessToken(userId, resUser.username, resRole?.roleEn ?? string.Empty);
                var refreshTokenValue = _jwtService.GenerateRefreshToken();

                refreshToken = new Refreshtoken
                {
                    Id = _systemService.GenGUID(),
                    UserId = userId,
                    Token = refreshTokenValue,
                    ExpiryDate = DateTime.Now.AddDays(expirationMinutes),
                    IsRevoked = false
                };
                _dbContexts.Refreshtokens.Add(refreshToken);
                await _dbContexts.SaveChangesAsync();

                AccessToken = accessToken;
                RefreshToken = refreshTokenValue;
                #endregion
            }

            return StatusCode(200, new { success = true, results = new { response, AccessToken, RefreshToken }, message = "Login successful" });
        }
        catch (Exception ex)
        {
            _dbContexts.Dispose();
            return StatusCode(200, new { status = false, message = ex.Message, error = ex.InnerException?.Message });
        }
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequestDto request)
    {
        try
        {
            var storedToken = await _dbContexts.Refreshtokens.FirstOrDefaultAsync(t => t.Token == request.RefreshToken);

            if (storedToken == null)
            {
                return Unauthorized(new { message = "Token ไม่ถูกต้อง" });
            }

            // [REUSE DETECTION] ถ้า Token นี้ถูกยกเลิกไปแล้ว แต่ยังมีคนนำกลับมาใช้ แสดงว่าโดนแฮก!
            if (storedToken.IsRevoked)
            {
                var userTokens = await _dbContexts.Refreshtokens
                    .Where(t => t.UserId == storedToken.UserId && !t.IsRevoked)
                    .ToListAsync();

                foreach (var token in userTokens)
                {
                    token.IsRevoked = true;
                    token.RevokedDate = DateTime.Now;
                }

                await _dbContexts.SaveChangesAsync();
                return Unauthorized(new { message = "พบความเสี่ยงด้านความปลอดภัย กรุณาเข้าสู่ระบบใหม่" });
            }

            // ตรวจสอบวันหมดอายุ
            if (storedToken.ExpiryDate < DateTime.Now)
            {
                return Unauthorized(new { message = "Refresh Token หมดอายุแล้ว" });
            }

            var resRole = await _dpUnitOfWork.Auths.GetListRole(storedToken.UserId);
            // [ROTATION PROCESS] ยกเลิก Token ใบเก่า
            var newRefreshTokenValue = _jwtService.GenerateRefreshToken();

            storedToken.IsRevoked = true;
            storedToken.RevokedDate = DateTime.Now;
            storedToken.ReplacedToken = newRefreshTokenValue;

            // ออก Token ใบใหม่
            var newAccessToken = _jwtService.GenerateAccessToken(storedToken.UserId, "admin", resRole?.roleEn);

            var expirationMinutes = double.Parse(_config["JwtSettings:RefreshTokenExpirationDays"] ?? "3");
            // var accessToken = _jwtService.GenerateAccessToken(userId, resUser.username, resRole?.roleEn ?? string.Empty);
            var refreshTokenValue = _jwtService.GenerateRefreshToken();

            var newRefreshToken = new Refreshtoken
            {
                Id = _systemService.GenGUID(),
                UserId = storedToken.UserId,
                Token = newRefreshTokenValue,
                ExpiryDate = DateTime.Now.AddDays(expirationMinutes),
                IsRevoked = false
            };

            _dbContexts.Refreshtokens.Add(newRefreshToken);
            await _dbContexts.SaveChangesAsync();

            return Ok(new AuthResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshTokenValue
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "เกิดข้อผิดพลาดในการรีเฟรชโทเค็น", error = ex.Message });
        }
    }
}
