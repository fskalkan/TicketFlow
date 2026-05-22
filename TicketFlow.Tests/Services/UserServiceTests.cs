using AutoMapper;
using FluentAssertions;
using Moq;
using TicketFlow.Application.DTOs.Users;
using TicketFlow.Application.Exceptions;
using TicketFlow.Application.Interfaces.Repositories;
using TicketFlow.Application.Services;
using TicketFlow.Domain.Entities;
using Xunit;

namespace TicketFlow.Tests.Services;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _mapperMock = new Mock<IMapper>();

        _userService = new UserService(
            _userRepositoryMock.Object,
            _mapperMock.Object
        );
    }

    [Fact]
    public async Task GetCurrentUserAsync_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        var userId = 1;

        var user = new User
        {
            Id = userId,
            FullName = "Samet Kalkan",
            Email = "samet@test.com",
            IsActive = true
        };

        var responseDto = new UserResponseDto
        {
            Id = userId,
            FullName = "Samet Kalkan",
            Email = "samet@test.com",
            IsActive = true
        };

        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync(user);

        _mapperMock
            .Setup(m => m.Map<UserResponseDto>(user))
            .Returns(responseDto);

        // Act
        var result = await _userService.GetCurrentUserAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(userId);
        result.FullName.Should().Be("Samet Kalkan");
        result.Email.Should().Be("samet@test.com");
        result.IsActive.Should().BeTrue();

        _userRepositoryMock.Verify(r => r.GetByIdAsync(userId), Times.Once);
    }

    [Fact]
    public async Task GetCurrentUserAsync_ShouldThrowNotFoundException_WhenUserDoesNotExist()
    {
        // Arrange
        var userId = 99;

        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync((User?)null);

        // Act
        var act = async () => await _userService.GetCurrentUserAsync(userId);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("User not found.");

        _userRepositoryMock.Verify(r => r.GetByIdAsync(userId), Times.Once);
    }

    [Fact]
    public async Task UpdateCurrentUserAsync_ShouldUpdateUser_WhenDataIsValid()
    {
        // Arrange
        var userId = 1;

        var user = new User
        {
            Id = userId,
            FullName = "Old Name",
            Email = "old@test.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("oldpassword"),
            IsActive = true
        };

        var updateDto = new UpdateUserDto
        {
            FullName = "Updated Name",
            Email = "UPDATED@TEST.COM",
            Password = "newpassword"
        };

        var responseDto = new UserResponseDto
        {
            Id = userId,
            FullName = "Updated Name",
            Email = "updated@test.com",
            IsActive = true
        };

        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync(user);

        _userRepositoryMock
            .Setup(r => r.EmailExistsAsync("updated@test.com"))
            .ReturnsAsync(false);

        _userRepositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<User>()))
            .Returns(Task.CompletedTask);

        _mapperMock
            .Setup(m => m.Map<UserResponseDto>(user))
            .Returns(responseDto);

        // Act
        var result = await _userService.UpdateCurrentUserAsync(userId, updateDto);

        // Assert
        result.Should().NotBeNull();
        result.FullName.Should().Be("Updated Name");
        result.Email.Should().Be("updated@test.com");

        user.FullName.Should().Be("Updated Name");
        user.Email.Should().Be("updated@test.com");
        BCrypt.Net.BCrypt.Verify("newpassword", user.PasswordHash).Should().BeTrue();

        _userRepositoryMock.Verify(r => r.GetByIdAsync(userId), Times.Once);
        _userRepositoryMock.Verify(r => r.EmailExistsAsync("updated@test.com"), Times.Once);
        _userRepositoryMock.Verify(r => r.UpdateAsync(user), Times.Once);
    }

    [Fact]
    public async Task UpdateCurrentUserAsync_ShouldThrowNotFoundException_WhenUserDoesNotExist()
    {
        // Arrange
        var userId = 99;

        var updateDto = new UpdateUserDto
        {
            FullName = "Updated Name",
            Email = "updated@test.com",
            Password = "newpassword"
        };

        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync((User?)null);

        // Act
        var act = async () => await _userService.UpdateCurrentUserAsync(userId, updateDto);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("User not found.");

        _userRepositoryMock.Verify(r => r.GetByIdAsync(userId), Times.Once);

        _userRepositoryMock.Verify(
            r => r.UpdateAsync(It.IsAny<User>()),
            Times.Never
        );
    }

    [Fact]
    public async Task UpdateCurrentUserAsync_ShouldThrowBadRequestException_WhenEmailAlreadyExists()
    {
        // Arrange
        var userId = 1;

        var user = new User
        {
            Id = userId,
            FullName = "Samet Kalkan",
            Email = "old@test.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
            IsActive = true
        };

        var updateDto = new UpdateUserDto
        {
            Email = "used@test.com"
        };

        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync(user);

        _userRepositoryMock
            .Setup(r => r.EmailExistsAsync("used@test.com"))
            .ReturnsAsync(true);

        // Act
        var act = async () => await _userService.UpdateCurrentUserAsync(userId, updateDto);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>()
            .WithMessage("Email address is already in use.");

        _userRepositoryMock.Verify(r => r.GetByIdAsync(userId), Times.Once);
        _userRepositoryMock.Verify(r => r.EmailExistsAsync("used@test.com"), Times.Once);

        _userRepositoryMock.Verify(
            r => r.UpdateAsync(It.IsAny<User>()),
            Times.Never
        );
    }

    [Fact]
    public async Task UpdateCurrentUserAsync_ShouldNotCheckEmailExists_WhenEmailIsSame()
    {
        // Arrange
        var userId = 1;

        var user = new User
        {
            Id = userId,
            FullName = "Old Name",
            Email = "same@test.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
            IsActive = true
        };

        var updateDto = new UpdateUserDto
        {
            FullName = "Updated Name",
            Email = "SAME@TEST.COM"
        };

        var responseDto = new UserResponseDto
        {
            Id = userId,
            FullName = "Updated Name",
            Email = "same@test.com",
            IsActive = true
        };

        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync(user);

        _userRepositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<User>()))
            .Returns(Task.CompletedTask);

        _mapperMock
            .Setup(m => m.Map<UserResponseDto>(user))
            .Returns(responseDto);

        // Act
        var result = await _userService.UpdateCurrentUserAsync(userId, updateDto);

        // Assert
        result.Should().NotBeNull();
        result.Email.Should().Be("same@test.com");
        user.FullName.Should().Be("Updated Name");
        user.Email.Should().Be("same@test.com");

        _userRepositoryMock.Verify(r => r.GetByIdAsync(userId), Times.Once);

        _userRepositoryMock.Verify(
            r => r.EmailExistsAsync(It.IsAny<string>()),
            Times.Never
        );

        _userRepositoryMock.Verify(r => r.UpdateAsync(user), Times.Once);
    }

    [Fact]
    public async Task UpdateCurrentUserAsync_ShouldNotUpdateFullName_WhenFullNameIsEmpty()
    {
        // Arrange
        var userId = 1;

        var user = new User
        {
            Id = userId,
            FullName = "Old Name",
            Email = "old@test.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
            IsActive = true
        };

        var updateDto = new UpdateUserDto
        {
            FullName = "   "
        };

        var responseDto = new UserResponseDto
        {
            Id = userId,
            FullName = "Old Name",
            Email = "old@test.com",
            IsActive = true
        };

        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync(user);

        _userRepositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<User>()))
            .Returns(Task.CompletedTask);

        _mapperMock
            .Setup(m => m.Map<UserResponseDto>(user))
            .Returns(responseDto);

        // Act
        var result = await _userService.UpdateCurrentUserAsync(userId, updateDto);

        // Assert
        result.Should().NotBeNull();
        user.FullName.Should().Be("Old Name");

        _userRepositoryMock.Verify(r => r.GetByIdAsync(userId), Times.Once);
        _userRepositoryMock.Verify(r => r.UpdateAsync(user), Times.Once);
    }
}