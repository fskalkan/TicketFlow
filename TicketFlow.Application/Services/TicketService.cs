using AutoMapper;
using TicketFlow.Application.DTOs.Tickets;
using TicketFlow.Application.Exceptions;
using TicketFlow.Application.Interfaces.Repositories;
using TicketFlow.Application.Interfaces.Services;
using TicketFlow.Domain.Entities;

namespace TicketFlow.Application.Services
{
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly IMapper _mapper;

        public TicketService(ITicketRepository ticketRepository, IMapper mapper)
        {
            _ticketRepository = ticketRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TicketResponseDto>> GetMyTicketsAsync(int userId)
        {
            var tickets = await _ticketRepository.GetByUserIdAsync(userId);

            return _mapper.Map<IEnumerable<TicketResponseDto>>(tickets);
        }

        public async Task<TicketResponseDto> GetByIdAsync(int id, int userId)
        {
            var ticket = await _ticketRepository.GetByIdAsync(id);

            if (ticket is null)
                throw new NotFoundException("Ticket not found.");

            if (ticket.UserId != userId)
                throw new UnauthorizedException("You are not authorized to access this ticket.");

            return _mapper.Map<TicketResponseDto>(ticket);
        }

        public async Task<TicketResponseDto> CreateAsync(int userId, CreateTicketDto createTicketDto)
        {
            var ticket = _mapper.Map<Ticket>(createTicketDto);

            ticket.UserId = userId;
            ticket.CreatedDate = DateTime.UtcNow;
            ticket.IsDeleted = false;

            await _ticketRepository.AddAsync(ticket);

            return _mapper.Map<TicketResponseDto>(ticket);
        }

        public async Task<TicketResponseDto> UpdateAsync(int id, int userId, UpdateTicketDto updateTicketDto)
        {
            var ticket = await _ticketRepository.GetByIdAsync(id);

            if (ticket is null)
                throw new NotFoundException("Ticket not found.");

            if (ticket.UserId != userId)
                throw new UnauthorizedException("You are not authorized to update this ticket.");

            if (!string.IsNullOrWhiteSpace(updateTicketDto.Title))
                ticket.Title = updateTicketDto.Title.Trim();

            if (updateTicketDto.Description is not null)
                ticket.Description = updateTicketDto.Description.Trim();

            if (updateTicketDto.Status.HasValue)
                ticket.Status = updateTicketDto.Status.Value;

            if (updateTicketDto.Priority.HasValue)
                ticket.Priority = updateTicketDto.Priority.Value;

            ticket.UpdatedDate = DateTime.UtcNow;

            await _ticketRepository.UpdateAsync(ticket);

            return _mapper.Map<TicketResponseDto>(ticket);
        }

        public async Task DeleteAsync(int id, int userId)
        {
            var ticket = await _ticketRepository.GetByIdAsync(id);

            if (ticket is null)
                throw new NotFoundException("Ticket not found.");

            if (ticket.UserId != userId)
                throw new UnauthorizedException("You are not authorized to delete this ticket.");

            await _ticketRepository.DeleteAsync(ticket);
        }
    }
}