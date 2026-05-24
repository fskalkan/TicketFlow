using FluentValidation.TestHelper;
using TicketFlow.Application.DTOs.Tickets;
using TicketFlow.Application.Validators.Tickets;
using TicketFlow.Domain.Enums;
using Xunit;

namespace TicketFlow.Tests.UnitTests.Validators.Tickets
{
    public class UpdateTicketDtoValidatorTests
    {
        private readonly UpdateTicketDtoValidator _validator;

        public UpdateTicketDtoValidatorTests()
        {
            _validator = new UpdateTicketDtoValidator();
        }

        [Fact]
        public void Should_Not_Have_Error_When_All_Fields_Are_Null()
        {
            // Arrange
            var model = new UpdateTicketDto
            {
                Title = null,
                Description = null,
                Priority = null,
                Status = null
            };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Should_Have_Error_When_Title_Is_Empty()
        {
            // Arrange
            var model = new UpdateTicketDto
            {
                Title = ""
            };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Title);
        }

        [Fact]
        public void Should_Have_Error_When_Title_Is_Too_Short()
        {
            // Arrange
            var model = new UpdateTicketDto
            {
                Title = "A"
            };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Title);
        }

        [Fact]
        public void Should_Have_Error_When_Title_Exceeds_100_Characters()
        {
            // Arrange
            var model = new UpdateTicketDto
            {
                Title = new string('A', 101)
            };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Title);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Title_Is_Valid()
        {
            // Arrange
            var model = new UpdateTicketDto
            {
                Title = "Updated ticket title"
            };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Title);
        }

        [Fact]
        public void Should_Have_Error_When_Description_Exceeds_1000_Characters()
        {
            // Arrange
            var model = new UpdateTicketDto
            {
                Description = new string('A', 1001)
            };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Description);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Description_Is_Valid()
        {
            // Arrange
            var model = new UpdateTicketDto
            {
                Description = "Updated description"
            };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Description);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Priority_Is_Valid()
        {
            // Arrange
            var model = new UpdateTicketDto
            {
                Priority = TicketPriority.Medium
            };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Priority);
        }

        [Fact]
        public void Should_Have_Error_When_Priority_Is_Invalid()
        {
            // Arrange
            var model = new UpdateTicketDto
            {
                Priority = (TicketPriority)99
            };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Priority);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Status_Is_Valid()
        {
            // Arrange
            var model = new UpdateTicketDto
            {
                Status = TicketStatus.Open
            };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Status);
        }

        [Fact]
        public void Should_Have_Error_When_Status_Is_Invalid()
        {
            // Arrange
            var model = new UpdateTicketDto
            {
                Status = (TicketStatus)99
            };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Status);
        }

        [Fact]
        public void Should_Not_Have_Error_When_UpdateTicketDto_Is_Valid()
        {
            // Arrange
            var model = new UpdateTicketDto
            {
                Title = "Updated ticket title",
                Description = "Updated ticket description",
                Priority = TicketPriority.High,
                Status = TicketStatus.InProgress
            };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}