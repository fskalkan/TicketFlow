using TicketFlow.Domain.Enums;

namespace TicketFlow.Application.DTOs.Tickets
{
    public class UpdateTicketDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public TicketStatus? Status { get; set; }
        public TicketPriority? Priority { get; set; }
    }
}
