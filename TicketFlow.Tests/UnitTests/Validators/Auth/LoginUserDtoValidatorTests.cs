using FluentValidation.TestHelper;
using TicketFlow.Application.DTOs.Auth;
using TicketFlow.Application.Validators.Auth;
using Xunit;

namespace TicketFlow.Tests.UnitTests.Validators.Auth
{
    public class LoginUserDtoValidatorTests
    {
        private readonly LoginUserDtoValidator _validator;

        public LoginUserDtoValidatorTests()
        {
            _validator = new LoginUserDtoValidator();
        }

        [Fact]
        public void Should_Have_Error_When_Email_Is_Empty()
        {
            // Arrange
            var model = new LoginUserDto
            {
                Email = "",
                Password = "123456"
            };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Fact]
        public void Should_Have_Error_When_Email_Format_Is_Invalid()
        {
            // Arrange
            var model = new LoginUserDto
            {
                Email = "invalid-email",
                Password = "123456"
            };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Fact]
        public void Should_Have_Error_When_Password_Is_Empty()
        {
            // Arrange
            var model = new LoginUserDto
            {
                Email = "test@example.com",
                Password = ""
            };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Password);
        }

        [Fact]
        public void Should_Not_Have_Error_When_LoginUserDto_Is_Valid()
        {
            // Arrange
            var model = new LoginUserDto
            {
                Email = "test@example.com",
                Password = "123456"
            };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}