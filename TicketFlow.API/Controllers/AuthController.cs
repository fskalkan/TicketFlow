using Microsoft.AspNetCore.Mvc;
using TicketFlow.Application.DTOs.Auth;
using TicketFlow.Application.Interfaces.Services;

namespace TicketFlow.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto registerUserDto)
        {
            var result = await _authService.RegisterAsync(registerUserDto);

            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDto loginUserDto)
        {
            var result = await _authService.LoginAsync(loginUserDto);

            return Ok(result);
        }
    }
}