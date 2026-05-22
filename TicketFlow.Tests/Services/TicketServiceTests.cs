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
}