using TicketFlow.Domain.Enums;

namespace TicketFlow.Application.DTOs.Tickets
{
    public class CreateTicketDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public TicketPriority Priority { get; set; } = TicketPriority.Medium;
    }
}
