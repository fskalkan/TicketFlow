using TicketFlow.Application.DTOs.Tickets;

namespace TicketFlow.Application.Interfaces.Services
{
    public interface ITicketService
    {
        Task<IEnumerable<TicketResponseDto>> GetMyTicketsAsync(int userId);
        Task<TicketResponseDto> GetByIdAsync(int id, int userId);
        Task<TicketResponseDto> CreateAsync(int userId, CreateTicketDto createTicketDto);
        Task<TicketResponseDto> UpdateAsync(int id, int userId, UpdateTicketDto updateTicketDto);
        Task DeleteAsync(int id, int userId);
    }
}