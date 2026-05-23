using AutoMapper;
using FluentAssertions;
using Moq;
using TicketFlow.Application.DTOs.Auth;
using TicketFlow.Application.DTOs.Users;
using TicketFlow.Application.Exceptions;
using TicketFlow.Application.Interfaces.Repositories;
using TicketFlow.Application.Interfaces.Services;
using TicketFlow.Application.Services;
using TicketFlow.Domain.Entities;
using Xunit;

namespace TicketFlow.Tests.UnitTests.Services;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IJwtService> _jwtServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _jwtServiceMock = new Mock<IJwtService>();
        _mapperMock = new Mock<IMapper>();

        _authService = new AuthService(
            _userRepositoryMock.Object,
            _jwtServiceMock.Object,
            _mapperMock.Object
        );
    }

    [Fact]
    public async Task RegisterAsync_ShouldRegisterUser_WhenEmailIsNotInUse()
    {
        // Arrange
        var registerDto = new RegisterUserDto
        {
            FullName = "Samet Kalkan",
            Email = "SAMET@TEST.COM",
            Password = "123456"
        };

        var expectedEmail = "samet@test.com";

        var responseDto = new UserResponseDto
        {
            Id = 1,
            FullName = "Samet Kalkan",
            Email = expectedEmail,
            IsActive = true
        };

        _userRepositoryMock
            .Setup(r => r.EmailExistsAsync(expectedEmail))
            .ReturnsAsync(false);

        _userRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<User>()))
            .Returns(Task.CompletedTask);

        _mapperMock
            .Setup(m => m.Map<UserResponseDto>(It.IsAny<User>()))
            .Returns(responseDto);

        // Act
        var result = await _authService.RegisterAsync(registerDto);

        // Assert
        result.Should().NotBeNull();
        result.Email.Should().Be(expectedEmail);
        result.FullName.Should().Be("Samet Kalkan");
        result.IsActive.Should().BeTrue();

        _userRepositoryMock.Verify(r => r.EmailExistsAsync(expectedEmail), Times.Once);
        _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_ShouldThrowBadRequestException_WhenEmailAlreadyExists()
    {
        // Arrange
        var registerDto = new RegisterUserDto
        {
            FullName = "Samet Kalkan",
            Email = "samet@test.com",
            Password = "123456"
        };

        var expectedEmail = "samet@test.com";

        _userRepositoryMock
            .Setup(r => r.EmailExistsAsync(expectedEmail))
            .ReturnsAsync(true);

        // Act
        var act = async () => await _authService.RegisterAsync(registerDto);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>()
            .WithMessage("Email address is already in use.");

        _userRepositoryMock.Verify(r => r.EmailExistsAsync(expectedEmail), Times.Once);

        _userRepositoryMock.Verify(
            r => r.AddAsync(It.IsAny<User>()),
            Times.Never
        );
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnTokenAndUser_WhenCredentialsAreValid()
    {
        // Arrange
        var password = "123456";
        var email = "samet@test.com";

        var loginDto = new LoginUserDto
        {
            Email = "SAMET@TEST.COM",
            Password = password
        };

        var user = new User
        {
            Id = 1,
            FullName = "Samet Kalkan",
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            IsActive = true
        };

        var userResponseDto = new UserResponseDto
        {
            Id = 1,
            FullName = "Samet Kalkan",
            Email = email,
            IsActive = true
        };

        var token = "fake-jwt-token";

        _userRepositoryMock
            .Setup(r => r.GetByEmailAsync(email))
            .ReturnsAsync(user);

        _jwtServiceMock
            .Setup(j => j.GenerateToken(user))
            .Returns(token);

        _mapperMock
            .Setup(m => m.Map<UserResponseDto>(user))
            .Returns(userResponseDto);

        // Act
        var result = await _authService.LoginAsync(loginDto);

        // Assert
        result.Should().NotBeNull();
        result.Token.Should().Be(token);
        result.User.Should().NotBeNull();
        result.User.Email.Should().Be(email);
        result.User.FullName.Should().Be("Samet Kalkan");

        _userRepositoryMock.Verify(r => r.GetByEmailAsync(email), Times.Once);
        _jwtServiceMock.Verify(j => j.GenerateToken(user), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_ShouldThrowUnauthorizedException_WhenUserDoesNotExist()
    {
        // Arrange
        var loginDto = new LoginUserDto
        {
            Email = "notfound@test.com",
            Password = "123456"
        };

        var expectedEmail = "notfound@test.com";

        _userRepositoryMock
            .Setup(r => r.GetByEmailAsync(expectedEmail))
            .ReturnsAsync((User?)null);

        // Act
        var act = async () => await _authService.LoginAsync(loginDto);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedException>()
            .WithMessage("Invalid email or password.");

        _userRepositoryMock.Verify(r => r.GetByEmailAsync(expectedEmail), Times.Once);

        _jwtServiceMock.Verify(
            j => j.GenerateToken(It.IsAny<User>()),
            Times.Never
        );
    }

    [Fact]
    public async Task LoginAsync_ShouldThrowUnauthorizedException_WhenPasswordIsInvalid()
    {
        // Arrange
        var correctPassword = "123456";
        var wrongPassword = "wrong-password";
        var email = "samet@test.com";

        var loginDto = new LoginUserDto
        {
            Email = email,
            Password = wrongPassword
        };

        var user = new User
        {
            Id = 1,
            FullName = "Samet Kalkan",
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(correctPassword),
            IsActive = true
        };

        _userRepositoryMock
            .Setup(r => r.GetByEmailAsync(email))
            .ReturnsAsync(user);

        // Act
        var act = async () => await _authService.LoginAsync(loginDto);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedException>()
            .WithMessage("Invalid email or password.");

        _userRepositoryMock.Verify(r => r.GetByEmailAsync(email), Times.Once);

        _jwtServiceMock.Verify(
            j => j.GenerateToken(It.IsAny<User>()),
            Times.Never
        );
    }

    [Fact]
    public async Task LoginAsync_ShouldThrowUnauthorizedException_WhenUserIsNotActive()
    {
        // Arrange
        var password = "123456";
        var email = "samet@test.com";

        var loginDto = new LoginUserDto
        {
            Email = email,
            Password = password
        };

        var user = new User
        {
            Id = 1,
            FullName = "Samet Kalkan",
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            IsActive = false
        };

        _userRepositoryMock
            .Setup(r => r.GetByEmailAsync(email))
            .ReturnsAsync(user);

        // Act
        var act = async () => await _authService.LoginAsync(loginDto);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedException>()
            .WithMessage("User account is not active.");

        _userRepositoryMock.Verify(r => r.GetByEmailAsync(email), Times.Once);

        _jwtServiceMock.Verify(
            j => j.GenerateToken(It.IsAny<User>()),
            Times.Never
        );
    }
}