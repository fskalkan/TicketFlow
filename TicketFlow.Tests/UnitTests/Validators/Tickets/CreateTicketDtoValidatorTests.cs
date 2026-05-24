using FluentValidation.TestHelper;
using TicketFlow.Application.DTOs.Tickets;
using TicketFlow.Application.Validators.Tickets;
using TicketFlow.Domain.Enums;
using Xunit;

namespace TicketFlow.Tests.UnitTests.Validators.Tickets
{
    public class CreateTicketDtoValidatorTests
    {
        private readonly CreateTicketDtoValidator _validator;

        public CreateTicketDtoValidatorTests()
        {
            _validator = new CreateTicketDtoValidator();
        }

        [Fact]
        public void Should_Have_Error_When_Title_Is_Empty()
        {
            var model = new CreateTicketDto
            {
                Title = "",
                Description = "Test description",
                Priority = TicketPriority.Medium
            };

            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.Title);
        }

        [Fact]
        public void Should_Have_Error_When_Title_Is_Too_Short()
        {
            var model = new CreateTicketDto
            {
                Title = "A",
                Description = "Test description",
                Priority = TicketPriority.Medium
            };

            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.Title);
        }

        [Fact]
        public void Should_Have_Error_When_Title_Exceeds_100_Characters()
        {
            var model = new CreateTicketDto
            {
                Title = new string('A', 101),
                Description = "Test description",
                Priority = TicketPriority.Medium
            };

            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.Title);
        }

        [Fact]
        public void Should_Have_Error_When_Description_Exceeds_1000_Characters()
        {
            var model = new CreateTicketDto
            {
                Title = "Valid title",
                Description = new string('A', 1001),
                Priority = TicketPriority.Medium
            };

            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.Description);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Model_Is_Valid()
        {
            var model = new CreateTicketDto
            {
                Title = "Valid title",
                Description = "Valid description",
                Priority = TicketPriority.Medium
            };

            var result = _validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}