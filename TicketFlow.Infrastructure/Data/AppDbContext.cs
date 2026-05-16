using Microsoft.EntityFrameworkCore;
using TicketFlow.Domain.Entities;

namespace TicketFlow.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Ticket> Tickets { get; set; } = null!;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FullName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.HasMany(e => e.Tickets)
                      .WithOne(t => t.User)
                      .HasForeignKey(t => t.UserId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Ticket>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.Status).IsRequired();
                entity.Property(e => e.Priority).IsRequired();
                entity.Property(e => e.CreatedDate).IsRequired();
                entity.Property(e => e.UpdatedDate).IsRequired(false);
                entity.Property(e => e.IsDeleted).IsRequired();
            });
        }
    }
}
