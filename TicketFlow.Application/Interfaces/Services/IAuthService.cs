using TicketFlow.Application.DTOs.Auth;
using TicketFlow.Application.DTOs.Users;

namespace TicketFlow.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<UserResponseDto> RegisterAsync(RegisterUserDto registerUserDto);
        Task<LoginResponseDto> LoginAsync(LoginUserDto loginUserDto);
    }
}
