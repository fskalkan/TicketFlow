using AutoMapper;
using FluentAssertions;
using Moq;
using TicketFlow.Application.DTOs.Tickets;
using TicketFlow.Application.Exceptions;
using TicketFlow.Application.Interfaces.Repositories;
using TicketFlow.Application.Services;
using TicketFlow.Domain.Entities;
using TicketFlow.Domain.Enums;
using Xunit;

namespace TicketFlow.Tests.Services;

public class TicketServiceTests
{
    private readonly Mock<ITicketRepository> _ticketRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly TicketService _ticketService;

    public TicketServiceTests()
    {
        _ticketRepositoryMock = new Mock<ITicketRepository>();
        _mapperMock = new Mock<IMapper>();

        _ticketService = new TicketService(
            _ticketRepositoryMock.Object,
            _mapperMock.Object
        );
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateTicket_WhenDataIsValid()
    {
        // Arrange
        var userId = 1;

        var createTicketDto = new CreateTicketDto
        {
            Title = "Test Ticket",
            Description = "Test Description",
            Priority = TicketPriority.Medium
        };

        var ticket = new Ticket
        {
            Title = createTicketDto.Title,
            Description = createTicketDto.Description,
            Priority = createTicketDto.Priority
        };

        var responseDto = new TicketResponseDto
        {
            Id = 1,
            UserId = userId,
            Title = "Test Ticket",
            Description = "Test Description",
            Priority = TicketPriority.Medium.ToString()
        };

        _mapperMock
            .Setup(m => m.Map<Ticket>(createTicketDto))
            .Returns(ticket);

        _ticketRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Ticket>()))
            .Returns(Task.CompletedTask);

        _mapperMock
            .Setup(m => m.Map<TicketResponseDto>(ticket))
            .Returns(responseDto);

        // Act
        var result = await _ticketService.CreateAsync(userId, createTicketDto);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be("Test Ticket");
        result.UserId.Should().Be(userId);

        _ticketRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Ticket>()), Times.Once);
    }


    [Fact]
    public async Task GetByIdAsync_ShouldThrowNotFoundException_WhenTicketDoesNotExist()
    {
        // Arrange
        var ticketId = 99;
        var userId = 1;

        _ticketRepositoryMock
            .Setup(r => r.GetByIdAsync(ticketId))
            .ReturnsAsync((Ticket?)null);

        // Act
        var act = async () => await _ticketService.GetByIdAsync(ticketId, userId);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Ticket not found.");

        _ticketRepositoryMock.Verify(r => r.GetByIdAsync(ticketId), Times.Once);
    }


    [Fact]
    public async Task GetByIdAsync_ShouldThrowUnauthorizedException_WhenUserDoesNotOwnTicket()
    {
        // Arrange
        var ticketId = 1;
        var currentUserId = 1;
        var ticketOwnerUserId = 2;

        var ticket = new Ticket
        {
            Id = ticketId,
            UserId = ticketOwnerUserId,
            Title = "Another User Ticket",
            Description = "This ticket belongs to another user"
        };

        _ticketRepositoryMock
            .Setup(r => r.GetByIdAsync(ticketId))
            .ReturnsAsync(ticket);

        // Act
        var act = async () => await _ticketService.GetByIdAsync(ticketId, currentUserId);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedException>()
            .WithMessage("You are not authorized to access this ticket.");

        _ticketRepositoryMock.Verify(r => r.GetByIdAsync(ticketId), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnTicket_WhenTicketExistsAndUserOwnsTicket()
    {
        // Arrange
        var ticketId = 1;
        var userId = 1;

        var ticket = new Ticket
        {
            Id = ticketId,
            UserId = userId,
            Title = "My Ticket",
            Description = "My ticket description"
        };

        var responseDto = new TicketResponseDto
        {
            Id = ticketId,
            UserId = userId,
            Title = "My Ticket",
            Description = "My ticket description"
        };

        _ticketRepositoryMock
            .Setup(r => r.GetByIdAsync(ticketId))
            .ReturnsAsync(ticket);

        _mapperMock
            .Setup(m => m.Map<TicketResponseDto>(ticket))
            .Returns(responseDto);

        // Act
        var result = await _ticketService.GetByIdAsync(ticketId, userId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(ticketId);
        result.UserId.Should().Be(userId);
        result.Title.Should().Be("My Ticket");

        _ticketRepositoryMock.Verify(r => r.GetByIdAsync(ticketId), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateTicket_WhenTicketExistsAndUserOwnsTicket()
    {
        // Arrange
        var ticketId = 1;
        var userId = 1;

        var existingTicket = new Ticket
        {
            Id = ticketId,
            UserId = userId,
            Title = "Old Title",
            Description = "Old Description",
            Status = TicketStatus.Open,
            Priority = TicketPriority.Low
        };

        var updateDto = new UpdateTicketDto
        {
            Title = "Updated Title",
            Description = "Updated Description",
            Status = TicketStatus.InProgress,
            Priority = TicketPriority.High
        };

        var responseDto = new TicketResponseDto
        {
            Id = ticketId,
            UserId = userId,
            Title = "Updated Title",
            Description = "Updated Description",
            Status = TicketStatus.InProgress.ToString(),
            Priority = TicketPriority.High.ToString()
        };

        _ticketRepositoryMock
            .Setup(r => r.GetByIdAsync(ticketId))
            .ReturnsAsync(existingTicket);

        _ticketRepositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<Ticket>()))
            .Returns(Task.CompletedTask);

        _mapperMock
            .Setup(m => m.Map<TicketResponseDto>(existingTicket))
            .Returns(responseDto);

        // Act
        var result = await _ticketService.UpdateAsync(ticketId, userId, updateDto);

        // Assert
        result.Should().NotBeNull();

        result.Id.Should().Be(ticketId);
        result.UserId.Should().Be(userId);
        result.Title.Should().Be("Updated Title");
        result.Description.Should().Be("Updated Description");
        result.Status.Should().Be(TicketStatus.InProgress.ToString());
        result.Priority.Should().Be(TicketPriority.High.ToString());

        existingTicket.Title.Should().Be("Updated Title");
        existingTicket.Description.Should().Be("Updated Description");
        existingTicket.Status.Should().Be(TicketStatus.InProgress);
        existingTicket.Priority.Should().Be(TicketPriority.High);
        existingTicket.UpdatedDate.Should().NotBeNull();

        _ticketRepositoryMock.Verify(r => r.GetByIdAsync(ticketId), Times.Once);
        _ticketRepositoryMock.Verify(r => r.UpdateAsync(existingTicket), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowNotFoundException_WhenTicketDoesNotExist()
    {
        // Arrange
        var ticketId = 99;
        var userId = 1;

        var updateDto = new UpdateTicketDto
        {
            Title = "Updated Title",
            Description = "Updated Description",
            Status = TicketStatus.InProgress,
            Priority = TicketPriority.High
        };

        _ticketRepositoryMock
            .Setup(r => r.GetByIdAsync(ticketId))
            .ReturnsAsync((Ticket?)null);

        // Act
        var act = async () => await _ticketService.UpdateAsync(ticketId, userId, updateDto);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Ticket not found.");

        _ticketRepositoryMock.Verify(r => r.GetByIdAsync(ticketId), Times.Once);

        _ticketRepositoryMock.Verify(
            r => r.UpdateAsync(It.IsAny<Ticket>()),
            Times.Never
        );
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowUnauthorizedException_WhenUserDoesNotOwnTicket()
    {
        // Arrange
        var ticketId = 1;
        var currentUserId = 1;
        var ticketOwnerUserId = 2;

        var existingTicket = new Ticket
        {
            Id = ticketId,
            UserId = ticketOwnerUserId,
            Title = "Old Title",
            Description = "Old Description",
            Status = TicketStatus.Open,
            Priority = TicketPriority.Low
        };

        var updateDto = new UpdateTicketDto
        {
            Title = "Updated Title",
            Description = "Updated Description",
            Status = TicketStatus.InProgress,
            Priority = TicketPriority.High
        };

        _ticketRepositoryMock
            .Setup(r => r.GetByIdAsync(ticketId))
            .ReturnsAsync(existingTicket);

        // Act
        var act = async () => await _ticketService.UpdateAsync(ticketId, currentUserId, updateDto);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedException>()
            .WithMessage("You are not authorized to update this ticket.");

        _ticketRepositoryMock.Verify(r => r.GetByIdAsync(ticketId), Times.Once);

        _ticketRepositoryMock.Verify(
            r => r.UpdateAsync(It.IsAny<Ticket>()),
            Times.Never
        );
    }

    [Fact]
    public async Task UpdateAsync_ShouldNotUpdateTitle_WhenTitleIsEmpty()
    {
        // Arrange
        var ticketId = 1;
        var userId = 1;

        var existingTicket = new Ticket
        {
            Id = ticketId,
            UserId = userId,
            Title = "Old Title",
            Description = "Old Description",
            Status = TicketStatus.Open,
            Priority = TicketPriority.Low
        };

        var updateDto = new UpdateTicketDto
        {
            Title = "   ",
            Description = "Updated Description",
            Status = TicketStatus.InProgress,
            Priority = TicketPriority.High
        };

        var responseDto = new TicketResponseDto
        {
            Id = ticketId,
            UserId = userId,
            Title = "Old Title",
            Description = "Updated Description",
            Status = TicketStatus.InProgress.ToString(),
            Priority = TicketPriority.High.ToString()
        };

        _ticketRepositoryMock
            .Setup(r => r.GetByIdAsync(ticketId))
            .ReturnsAsync(existingTicket);

        _ticketRepositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<Ticket>()))
            .Returns(Task.CompletedTask);

        _mapperMock
            .Setup(m => m.Map<TicketResponseDto>(existingTicket))
            .Returns(responseDto);

        // Act
        var result = await _ticketService.UpdateAsync(ticketId, userId, updateDto);

        // Assert
        result.Should().NotBeNull();

        existingTicket.Title.Should().Be("Old Title");
        existingTicket.Description.Should().Be("Updated Description");
        existingTicket.Status.Should().Be(TicketStatus.InProgress);
        existingTicket.Priority.Should().Be(TicketPriority.High);

        _ticketRepositoryMock.Verify(r => r.GetByIdAsync(ticketId), Times.Once);
        _ticketRepositoryMock.Verify(r => r.UpdateAsync(existingTicket), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteTicket_WhenTicketExistsAndUserOwnsTicket()
    {
        // Arrange
        var ticketId = 1;
        var userId = 1;

        var existingTicket = new Ticket
        {
            Id = ticketId,
            UserId = userId,
            Title = "Test Ticket",
            Description = "Test Description",
            Status = TicketStatus.Open,
            Priority = TicketPriority.Medium
        };

        _ticketRepositoryMock
            .Setup(r => r.GetByIdAsync(ticketId))
            .ReturnsAsync(existingTicket);

        _ticketRepositoryMock
            .Setup(r => r.DeleteAsync(It.IsAny<Ticket>()))
            .Returns(Task.CompletedTask);

        // Act
        await _ticketService.DeleteAsync(ticketId, userId);

        // Assert
        _ticketRepositoryMock.Verify(r => r.GetByIdAsync(ticketId), Times.Once);
        _ticketRepositoryMock.Verify(r => r.DeleteAsync(existingTicket), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrowNotFoundException_WhenTicketDoesNotExist()
    {
        // Arrange
        var ticketId = 99;
        var userId = 1;

        _ticketRepositoryMock
            .Setup(r => r.GetByIdAsync(ticketId))
            .ReturnsAsync((Ticket?)null);

        // Act
        var act = async () => await _ticketService.DeleteAsync(ticketId, userId);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Ticket not found.");

        _ticketRepositoryMock.Verify(r => r.GetByIdAsync(ticketId), Times.Once);

        _ticketRepositoryMock.Verify(
            r => r.DeleteAsync(It.IsAny<Ticket>()),
            Times.Never
        );
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrowUnauthorizedException_WhenUserDoesNotOwnTicket()
    {
        // Arrange
        var ticketId = 1;
        var currentUserId = 1;
        var ticketOwnerUserId = 2;

        var existingTicket = new Ticket
        {
            Id = ticketId,
            UserId = ticketOwnerUserId,
            Title = "Test Ticket",
            Description = "Test Description",
            Status = TicketStatus.Open,
            Priority = TicketPriority.Medium
        };

        _ticketRepositoryMock
            .Setup(r => r.GetByIdAsync(ticketId))
            .ReturnsAsync(existingTicket);

        // Act
        var act = async () => await _ticketService.DeleteAsync(ticketId, currentUserId);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedException>()
            .WithMessage("You are not authorized to delete this ticket.");

        _ticketRepositoryMock.Verify(r => r.GetByIdAsync(ticketId), Times.Once);

        _ticketRepositoryMock.Verify(
            r => r.DeleteAsync(It.IsAny<Ticket>()),
            Times.Never
        );
    }

    [Fact]
    public async Task GetMyTicketsAsync_ShouldReturnTickets_WhenUserHasTickets()
    {
        // Arrange
        var userId = 1;

        var tickets = new List<Ticket>
    {
        new Ticket
        {
            Id = 1,
            UserId = userId,
            Title = "First Ticket",
            Description = "First Description",
            Status = TicketStatus.Open,
            Priority = TicketPriority.Medium
        },
        new Ticket
        {
            Id = 2,
            UserId = userId,
            Title = "Second Ticket",
            Description = "Second Description",
            Status = TicketStatus.InProgress,
            Priority = TicketPriority.High
        }
    };

        var responseDtos = new List<TicketResponseDto>
    {
        new TicketResponseDto
        {
            Id = 1,
            UserId = userId,
            Title = "First Ticket",
            Description = "First Description",
            Status = TicketStatus.Open.ToString(),
            Priority = TicketPriority.Medium.ToString()
        },
        new TicketResponseDto
        {
            Id = 2,
            UserId = userId,
            Title = "Second Ticket",
            Description = "Second Description",
            Status = TicketStatus.InProgress.ToString(),
            Priority = TicketPriority.High.ToString()
        }
    };

        _ticketRepositoryMock
            .Setup(r => r.GetByUserIdAsync(userId))
            .ReturnsAsync(tickets);

        _mapperMock
            .Setup(m => m.Map<IEnumerable<TicketResponseDto>>(tickets))
            .Returns(responseDtos);

        // Act
        var result = await _ticketService.GetMyTicketsAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);

        result.First().Title.Should().Be("First Ticket");
        result.Last().Title.Should().Be("Second Ticket");

        _ticketRepositoryMock.Verify(r => r.GetByUserIdAsync(userId), Times.Once);
    }

    [Fact]
    public async Task GetMyTicketsAsync_ShouldReturnEmptyList_WhenUserHasNoTickets()
    {
        // Arrange
        var userId = 1;

        var tickets = new List<Ticket>();

        var responseDtos = new List<TicketResponseDto>();

        _ticketRepositoryMock
            .Setup(r => r.GetByUserIdAsync(userId))
            .ReturnsAsync(tickets);

        _mapperMock
            .Setup(m => m.Map<IEnumerable<TicketResponseDto>>(tickets))
            .Returns(responseDtos);

        // Act
        var result = await _ticketService.GetMyTicketsAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();

        _ticketRepositoryMock.Verify(r => r.GetByUserIdAsync(userId), Times.Once);
    }

    private static Ticket CreateTicket(
    int id = 1,
    int userId = 1,
    string title = "Test Ticket",
    string description = "Test Description",
    TicketStatus status = TicketStatus.Open,
    TicketPriority priority = TicketPriority.Medium)
    {
        return new Ticket
        {
            Id = id,
            UserId = userId,
            Title = title,
            Description = description,
            Status = status,
            Priority = priority
        };
    }
}