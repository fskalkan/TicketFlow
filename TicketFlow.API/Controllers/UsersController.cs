using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketFlow.Application.DTOs.Users;
using TicketFlow.Application.Exceptions;
using TicketFlow.Application.Interfaces.Services;

namespace TicketFlow.API.Controllers
{
    [Route("api/users")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userId = GetCurrentUserId();

            var result = await _userService.GetCurrentUserAsync(userId);

            return Ok(result);
        }

        [HttpPut("me")]
        public async Task<IActionResult> UpdateCurrentUser([FromBody] UpdateUserDto updateUserDto)
        {
            var userId = GetCurrentUserId();

            var result = await _userService.UpdateCurrentUserAsync(userId, updateUserDto);

            return Ok(result);
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(userIdClaim))
                throw new UnauthorizedException("User id not found in token.");

            if (!int.TryParse(userIdClaim, out var userId))
                throw new UnauthorizedException("Invalid user id in token.");

            return userId;
        }
    }
}