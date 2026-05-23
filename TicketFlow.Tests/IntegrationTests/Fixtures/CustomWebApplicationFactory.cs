using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TicketFlow.Infrastructure.Data;

namespace TicketFlow.Tests.IntegrationTests.Fixtures;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

            if (descriptor is not null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseInMemoryDatabase("TicketFlowTestDb");
            });

            var serviceProvider = services.BuildServiceProvider();

            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();
        });
    }
}