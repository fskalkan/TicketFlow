using FluentValidation.TestHelper;
using TicketFlow.Application.DTOs.Auth;
using TicketFlow.Application.Validators.Auth;
using Xunit;

namespace TicketFlow.Tests.UnitTests.Validators.Auth
{
    public class RegisterUserDtoValidatorTests
    {
        private readonly RegisterUserDtoValidator _validator;

        public RegisterUserDtoValidatorTests()
        {
            _validator = new RegisterUserDtoValidator();
        }

        [Fact]
        public void Should_Have_Error_When_FullName_Is_Empty()
        {
            // Arrange
            var model = new RegisterUserDto
            {
                FullName = "",
                Email = "test@example.com",
                Password = "123456"
            };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.FullName);
        }

        [Fact]
        public void Should_Have_Error_When_FullName_Is_Too_Short()
        {
            // Arrange
            var model = new RegisterUserDto
            {
                FullName = "A",
                Email = "test@example.com",
                Password = "123456"
            };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.FullName);
        }

        [Fact]
        public void Should_Have_Error_When_FullName_Exceeds_100_Characters()
        {
            // Arrange
            var model = new RegisterUserDto
            {
                FullName = new string('A', 101),
                Email = "test@example.com",
                Password = "123456"
            };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.FullName);
        }

        [Fact]
        public void Should_Have_Error_When_Email_Is_Empty()
        {
            // Arrange
            var model = new RegisterUserDto
            {
                FullName = "Test User",
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
            var model = new RegisterUserDto
            {
                FullName = "Test User",
                Email = "invalid-email",
                Password = "123456"
            };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Fact]
        public void Should_Have_Error_When_Email_Exceeds_100_Characters()
        {
            // Arrange
            var model = new RegisterUserDto
            {
                FullName = "Test User",
                Email = $"{new string('a', 246)}@test.com",
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
            var model = new RegisterUserDto
            {
                FullName = "Test User",
                Email = "test@example.com",
                Password = ""
            };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Password);
        }

        [Fact]
        public void Should_Have_Error_When_Password_Is_Too_Short()
        {
            // Arrange
            var model = new RegisterUserDto
            {
                FullName = "Test User",
                Email = "test@example.com",
                Password = "12345"
            };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Password);
        }

        [Fact]
        public void Should_Have_Error_When_Password_Exceeds_100_Characters()
        {
            // Arrange
            var model = new RegisterUserDto
            {
                FullName = "Test User",
                Email = "test@example.com",
                Password = new string('A', 101)
            };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Password);
        }

        [Fact]
        public void Should_Not_Have_Error_When_RegisterUserDto_Is_Valid()
        {
            // Arrange
            var model = new RegisterUserDto
            {
                FullName = "Test User",
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