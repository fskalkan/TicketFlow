using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketFlow.Application.DTOs.Tickets;
using TicketFlow.Application.Exceptions;
using TicketFlow.Application.Interfaces.Services;

namespace TicketFlow.API.Controllers
{
    [Route("api/tickets")]
    [ApiController]
    [Authorize]
    public class TicketsController : ControllerBase
    {
        private readonly ITicketService _ticketService;

        public TicketsController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        [HttpGet]
        public async Task<IActionResult> GetMyTickets()
        {
            var userId = GetCurrentUserId();

            var result = await _ticketService.GetMyTicketsAsync(userId);

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var userId = GetCurrentUserId();

            var result = await _ticketService.GetByIdAsync(id, userId);

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTicketDto createTicketDto)
        {
            var userId = GetCurrentUserId();

            var result = await _ticketService.CreateAsync(userId, createTicketDto);

            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateTicketDto updateTicketDto)
        {
            var userId = GetCurrentUserId();

            var result = await _ticketService.UpdateAsync(id, userId, updateTicketDto);

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetCurrentUserId();

            await _ticketService.DeleteAsync(id, userId);

            return NoContent();
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