using TicketFlow.Domain.Entities;

namespace TicketFlow.Application.Interfaces.Services
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}