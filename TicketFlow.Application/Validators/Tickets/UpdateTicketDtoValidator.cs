using FluentValidation;
using TicketFlow.Application.DTOs.Tickets;
using TicketFlow.Domain.Enums;

namespace TicketFlow.Application.Validators.Tickets
{
    public class UpdateTicketDtoValidator : AbstractValidator<UpdateTicketDto>
    {
        public UpdateTicketDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title cannot be empty.")
                .MinimumLength(2).WithMessage("Title must be at least 2 characters long.")
                .MaximumLength(100).WithMessage("Title must not exceed 100 characters.")
                .When(x => x.Title is not null);

            RuleFor(x => x.Description)
                .MaximumLength(1000)
                .WithMessage("Description must not exceed 1000 characters.")
                .When(x => x.Description is not null);

            RuleFor(x => x.Priority)
                .Must(priority =>
                    priority is null ||
                    Enum.GetValues<TicketPriority>().Contains(priority.Value))
                .WithMessage("Invalid priority value.");

            RuleFor(x => x.Status)
                .Must(status =>
                    status is null ||
                    Enum.GetValues<TicketStatus>().Contains(status.Value))
                .WithMessage("Invalid status value.");
        }
    }
}