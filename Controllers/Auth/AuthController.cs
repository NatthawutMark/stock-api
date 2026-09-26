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


    private readonly DapperUnitOfWork _dpUnitOfWork;
    private readonly IJwtService _jwtService;
    private readonly ISystemService _systemService;

    public AuthController(DapperUnitOfWork dpUnitOfWork, ISystemService systemService, IJwtService jwtService)
    {
        _dpUnitOfWork = dpUnitOfWork;
        _systemService = systemService;
        _jwtService = jwtService;
    }

    [HttpPost("login", Name = "Login")]
    public async Task<ActionResult> Login([FromBody] login req)
    {
        try
        {
            loginResponse response = new loginResponse();
            dynamic reqLogin = new ExpandoObject();
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

                var resRole = await _dpUnitOfWork.Auths.GetRole(resUser.userid);

                response = new loginResponse
                {
                    username = resUser.username,
                    fname = resUser.fname,
                    lname = resUser.lname,
                    role = resRole
                };

            }

            return StatusCode(200, new { status = true, data = response, message = "Login successful" });
        }
        catch (Exception ex)
        {
            return StatusCode(200, new { status = false, message = ex.Message, error = ex.InnerException?.Message });
        }
    }

    // [HttpPost("refresh")]
    // public async Task<IActionResult> Refresh([FromBody] RefreshRequestDto request)
    // {
    //     var storedToken = await _context.RefreshTokens
    //         .FirstOrDefaultAsync(t => t.Token == request.RefreshToken);

    //     if (storedToken == null)
    //     {
    //         return Unauthorized(new { message = "Token ไม่ถูกต้อง" });
    //     }

    //     // [REUSE DETECTION] ถ้า Token นี้ถูกยกเลิกไปแล้ว แต่ยังมีคนนำกลับมาใช้ แสดงว่าโดนแฮก!
    //     if (storedToken.IsRevoked)
    //     {
    //         var userTokens = await _context.RefreshTokens
    //             .Where(t => t.UserId == storedToken.UserId && !t.IsRevoked)
    //             .ToListAsync();

    //         foreach (var token in userTokens)
    //         {
    //             token.IsRevoked = true;
    //             token.RevokedDate = DateTime.UtcNow;
    //         }

    //         await _context.SaveChangesAsync();
    //         return Unauthorized(new { message = "พบความเสี่ยงด้านความปลอดภัย กรุณาเข้าสู่ระบบใหม่" });
    //     }

    //     // ตรวจสอบวันหมดอายุ
    //     if (storedToken.ExpiryDate < DateTime.UtcNow)
    //     {
    //         return Unauthorized(new { message = "Refresh Token หมดอายุแล้ว" });
    //     }

    //     // [ROTATION PROCESS] ยกเลิก Token ใบเก่า
    //     var newRefreshTokenValue = _jwtService.GenerateRefreshToken();

    //     storedToken.IsRevoked = true;
    //     storedToken.RevokedDate = DateTime.UtcNow;
    //     storedToken.ReplacedByToken = newRefreshTokenValue;

    //     // ออก Token ใบใหม่
    //     var newAccessToken = _jwtService.GenerateAccessToken(storedToken.UserId, "username", "Admin");
    //     var newRefreshToken = new RefreshToken
    //     {
    //         UserId = storedToken.UserId,
    //         Token = newRefreshTokenValue,
    //         ExpiryDate = DateTime.UtcNow.AddDays(7),
    //         IsRevoked = false
    //     };

    //     _context.RefreshTokens.Add(newRefreshToken);
    //     await _context.SaveChangesAsync();

    //     return Ok(new AuthResponseDto
    //     {
    //         AccessToken = newAccessToken,
    //         RefreshToken = newRefreshTokenValue
    //     });
    // }
}
