namespace TicketFlow.Domain.Entities
{
    public class User
    {
        public int Id{ get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    }
}
