using Microsoft.AspNetCore.Mvc;
using TicketFlow.Application.DTOs.Auth;
using TicketFlow.Application.Interfaces.Services;
using FluentValidation;

namespace TicketFlow.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IValidator<RegisterUserDto> _registerValidator;
        private readonly IValidator<LoginUserDto> _loginValidator;

        public AuthController(IAuthService authService, IValidator<RegisterUserDto> registerValidator, IValidator<LoginUserDto> loginValidator)
        {
            _authService = authService;
            _loginValidator = loginValidator;
            _registerValidator = registerValidator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto registerUserDto)
        {

            var validationResult = await _registerValidator.ValidateAsync(registerUserDto);

            if (!validationResult.IsValid)
            {
                return ValidationErrorResponse(validationResult);
            }

            var result = await _authService.RegisterAsync(registerUserDto);

            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDto loginUserDto)
        {

            var validationResult = await _loginValidator.ValidateAsync(loginUserDto);

            if (!validationResult.IsValid)
            {
                return ValidationErrorResponse(validationResult);
            }

            var result = await _authService.LoginAsync(loginUserDto);

            return Ok(result);
        }

        private IActionResult ValidationErrorResponse(FluentValidation.Results.ValidationResult validationResult)
        {
            var errors = validationResult.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray()
                );

            return BadRequest(new { Errors = errors });
        }
    }
}