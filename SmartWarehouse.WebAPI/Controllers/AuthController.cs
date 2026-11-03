using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartWarehouse.Application.Contracts.Services;
using SmartWarehouse.Application.DTOs.Auth;
using System.Threading.Tasks;

namespace SmartWarehouse.WebAPI.Controllers
{
    /// <summary>
    /// API Xác thực và Cấp phát JWT Bearer Token — ASM-U06-01
    /// </summary>
    [Route("api/v1/auth")]
    public class AuthController : BaseApiController
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Đăng nhập lấy Bearer Token (Tài khoản mẫu: manager/Manager123! hoặc staff/Staff123!)
        /// POST /api/v1/auth/login
        /// </summary>
        /// <param name="dto">Thông tin đăng nhập</param>
        /// <response code="200">Đăng nhập thành công, trả về JWT Token và Role.</response>
        /// <response code="401">Thông tin đăng nhập không chính xác.</response>
        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto dto)
        {
            var result = await _authService.AuthenticateAsync(dto);
            if (result is null)
            {
                return Unauthorized(new ProblemDetails
                {
                    Status = StatusCodes.Status401Unauthorized,
                    Title = "Unauthorized",
                    Detail = "Invalid username or password. Demo accounts: manager / Manager123! or staff / Staff123!"
                });
            }

            return Ok(result);
        }
    }
}
