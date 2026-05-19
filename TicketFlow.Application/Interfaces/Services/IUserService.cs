using TicketFlow.Application.DTOs.Users;

namespace TicketFlow.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<UserResponseDto> GetCurrentUserAsync(int userId);
        Task<UserResponseDto> UpdateCurrentUserAsync(int userId, UpdateUserDto updateUserDto);
    }
}
