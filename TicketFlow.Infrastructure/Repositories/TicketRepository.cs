using Microsoft.EntityFrameworkCore;
using TicketFlow.Application.Interfaces.Repositories;
using TicketFlow.Domain.Entities;
using TicketFlow.Infrastructure.Data;

namespace TicketFlow.Infrastructure.Repositories
{
    public class TicketRepository : ITicketRepository
    {
        private readonly AppDbContext _dbContext;

        public TicketRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(Ticket ticket)
        {
            await _dbContext.Tickets.AddAsync(ticket);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Ticket ticket)
        {
            ticket.IsDeleted = true;
            ticket.UpdatedDate = DateTime.UtcNow;

            _dbContext.Tickets.Update(ticket);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Ticket?> GetByIdAsync(int id)
        {
            return await _dbContext.Tickets.FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);
        }

        public async Task<IEnumerable<Ticket>> GetByUserIdAsync(int userId)
        {
            return await _dbContext.Tickets
                .Where(t => t.UserId == userId && !t.IsDeleted)
                .OrderByDescending(t => t.CreatedDate)
                .ToListAsync();
        }

        public async Task UpdateAsync(Ticket ticket)
        {
            _dbContext.Tickets.Update(ticket);
            await _dbContext.SaveChangesAsync();
        }
    }
}
