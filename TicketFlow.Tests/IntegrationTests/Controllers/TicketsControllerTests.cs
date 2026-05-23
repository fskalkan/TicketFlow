using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using TicketFlow.Application.DTOs.Auth;
using TicketFlow.Application.DTOs.Tickets;
using TicketFlow.Domain.Enums;
using TicketFlow.Tests.IntegrationTests.Fixtures;

namespace TicketFlow.Tests.IntegrationTests.Controllers;

public class TicketsControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public TicketsControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetMyTickets_Should_Return_Unauthorized_When_Token_Is_Missing()
    {
        // Act
        var response = await _client.GetAsync("/api/tickets");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateTicket_Should_Return_OK_When_Request_Is_Valid()
    {
        // Arrange
        await AuthenticateClientAsync();

        var request = CreateTicketRequest();

        // Act
        var response = await _client.PostAsJsonAsync("/api/tickets", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();

        json.GetProperty("id").GetInt32().Should().BeGreaterThan(0);
        json.GetProperty("title").GetString().Should().Be(request.Title);
        json.GetProperty("description").GetString().Should().Be(request.Description);
    }

    [Fact]
    public async Task GetMyTickets_Should_Return_OK_When_User_Is_Authenticated()
    {
        // Arrange
        await AuthenticateClientAsync();

        var createRequest = CreateTicketRequest();

        await _client.PostAsJsonAsync("/api/tickets", createRequest);

        // Act
        var response = await _client.GetAsync("/api/tickets");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();

        json.ValueKind.Should().Be(JsonValueKind.Array);
        json.GetArrayLength().Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GetById_Should_Return_OK_When_Ticket_Exists()
    {
        // Arrange
        await AuthenticateClientAsync();

        var ticketId = await CreateTicketAndGetIdAsync();

        // Act
        var response = await _client.GetAsync($"/api/tickets/{ticketId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();

        json.GetProperty("id").GetInt32().Should().Be(ticketId);
    }

    [Fact]
    public async Task UpdateTicket_Should_Return_OK_When_Ticket_Exists()
    {
        // Arrange
        await AuthenticateClientAsync();

        var ticketId = await CreateTicketAndGetIdAsync();

        var updateRequest = new UpdateTicketDto
        {
            Title = "Updated Ticket Title",
            Description = "Updated ticket description",
            Priority = TicketPriority.High,
            Status = TicketStatus.InProgress
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/tickets/{ticketId}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();

        json.GetProperty("id").GetInt32().Should().Be(ticketId);
        json.GetProperty("title").GetString().Should().Be(updateRequest.Title);
        json.GetProperty("description").GetString().Should().Be(updateRequest.Description);
    }

    [Fact]
    public async Task DeleteTicket_Should_Return_NoContent_When_Ticket_Exists()
    {
        // Arrange
        await AuthenticateClientAsync();

        var ticketId = await CreateTicketAndGetIdAsync();

        // Act
        var response = await _client.DeleteAsync($"/api/tickets/{ticketId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task GetById_Should_Return_NotFound_When_Ticket_Does_Not_Exist()
    {
        // Arrange
        await AuthenticateClientAsync();

        var nonExistingTicketId = 999999;

        // Act
        var response = await _client.GetAsync($"/api/tickets/{nonExistingTicketId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetById_Should_Return_NotFound_When_Ticket_Belongs_To_Another_User()
    {
        // Arrange
        await AuthenticateClientAsync();

        var firstUserTicketId = await CreateTicketAndGetIdAsync();

        await AuthenticateClientAsync();

        // Act
        var response = await _client.GetAsync($"/api/tickets/{firstUserTicketId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateTicket_Should_Return_NotFound_When_Ticket_Belongs_To_Another_User()
    {
        // Arrange
        await AuthenticateClientAsync();

        var firstUserTicketId = await CreateTicketAndGetIdAsync();

        await AuthenticateClientAsync();

        var updateRequest = new UpdateTicketDto
        {
            Title = "Another User Updated Title",
            Description = "Another user should not update this ticket",
            Priority = TicketPriority.Low,
            Status = TicketStatus.Closed
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/tickets/{firstUserTicketId}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteTicket_Should_Return_NotFound_When_Ticket_Belongs_To_Another_User()
    {
        // Arrange
        await AuthenticateClientAsync();

        var firstUserTicketId = await CreateTicketAndGetIdAsync();

        await AuthenticateClientAsync();

        // Act
        var response = await _client.DeleteAsync($"/api/tickets/{firstUserTicketId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetMyTickets_Should_Return_Only_Current_Users_Tickets()
    {
        // Arrange
        await AuthenticateClientAsync();

        var firstUserTicket = CreateTicketRequest("First User Ticket");
        await _client.PostAsJsonAsync("/api/tickets", firstUserTicket);

        await AuthenticateClientAsync();

        var secondUserTicket = CreateTicketRequest("Second User Ticket");
        await _client.PostAsJsonAsync("/api/tickets", secondUserTicket);

        // Act
        var response = await _client.GetAsync("/api/tickets");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();

        json.ValueKind.Should().Be(JsonValueKind.Array);

        var titles = json.EnumerateArray()
            .Select(x => x.GetProperty("title").GetString())
            .ToList();

        titles.Should().Contain(secondUserTicket.Title);
        titles.Should().NotContain(firstUserTicket.Title);
    }

    private async Task AuthenticateClientAsync()
    {
        var registerRequest = new RegisterUserDto
        {
            FullName = "Ticket Test User",
            Email = $"ticketuser_{Guid.NewGuid()}@test.com",
            Password = "123456"
        };

        await _client.PostAsJsonAsync("/api/auth/register", registerRequest);

        var loginRequest = new LoginUserDto
        {
            Email = registerRequest.Email,
            Password = registerRequest.Password
        };

        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await loginResponse.Content.ReadFromJsonAsync<JsonElement>();

        var token = json.GetProperty("token").GetString();

        token.Should().NotBeNullOrWhiteSpace();

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
    }

    private async Task<int> CreateTicketAndGetIdAsync()
    {
        var createRequest = CreateTicketRequest();

        var response = await _client.PostAsJsonAsync("/api/tickets", createRequest);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();

        return json.GetProperty("id").GetInt32();
    }

    private static CreateTicketDto CreateTicketRequest(string? title = null)
    {
        return new CreateTicketDto
        {
            Title = title ?? $"Test Ticket {Guid.NewGuid()}",
            Description = "Test ticket description",
            Priority = TicketPriority.Medium
        };
    }
}