using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using TicketFlow.Application.DTOs.Auth;
using TicketFlow.Tests.IntegrationTests.Fixtures;

namespace TicketFlow.Tests.IntegrationTests.Controllers;

public class AuthControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_Should_Return_OK_When_Request_Is_Valid()
    {
        // Arrange
        var request = CreateRegisterRequest();

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();

        json.GetProperty("id").GetInt32().Should().BeGreaterThan(0);
        json.GetProperty("fullName").GetString().Should().Be(request.FullName);
        json.GetProperty("email").GetString().Should().Be(request.Email);
    }

    [Fact]
    public async Task Register_Should_Return_BadRequest_When_Email_Already_Exists()
    {
        // Arrange
        var request = CreateRegisterRequest();

        await _client.PostAsJsonAsync("/api/auth/register", request);

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_Should_Return_OK_And_Token_When_Credentials_Are_Valid()
    {
        // Arrange
        var registerRequest = CreateRegisterRequest();

        await _client.PostAsJsonAsync("/api/auth/register", registerRequest);

        var loginRequest = new LoginUserDto
        {
            Email = registerRequest.Email,
            Password = registerRequest.Password
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();

        json.GetProperty("token").GetString().Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Login_Should_Return_Unauthorized_When_Password_Is_Wrong()
    {
        // Arrange
        var registerRequest = CreateRegisterRequest();

        await _client.PostAsJsonAsync("/api/auth/register", registerRequest);

        var loginRequest = new LoginUserDto
        {
            Email = registerRequest.Email,
            Password = "wrong-password"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_Should_Return_Unauthorized_When_Email_Does_Not_Exist()
    {
        // Arrange
        var loginRequest = new LoginUserDto
        {
            Email = $"notfound_{Guid.NewGuid()}@test.com",
            Password = "123456"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    private static RegisterUserDto CreateRegisterRequest()
    {
        return new RegisterUserDto
        {
            FullName = "Test User",
            Email = $"testuser_{Guid.NewGuid()}@test.com",
            Password = "123456"
        };
    }
}