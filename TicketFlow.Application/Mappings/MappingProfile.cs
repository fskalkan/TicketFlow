using AutoMapper;
using TicketFlow.Application.DTOs.Tickets;
using TicketFlow.Application.DTOs.Users;
using TicketFlow.Domain.Entities;
using TicketFlow.Domain.Enums;

namespace TicketFlow.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserResponseDto>();

            CreateMap<Ticket, TicketResponseDto>()
                .ForMember(dest => dest.Status,
                    opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.Priority,
                    opt => opt.MapFrom(src => src.Priority.ToString()));

            CreateMap<CreateTicketDto, Ticket>()
                .ForMember(dest => dest.Status,
                    opt => opt.MapFrom(src => TicketStatus.Open))
                .ForMember(dest => dest.CreatedDate,
                    opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.IsDeleted,
                    opt => opt.MapFrom(src => false));
        }
    }
}