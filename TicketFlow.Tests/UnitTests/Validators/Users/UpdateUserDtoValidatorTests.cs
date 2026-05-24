using FluentValidation.TestHelper;
using TicketFlow.Application.DTOs.Users;
using TicketFlow.Application.Validators.Users;
using Xunit;

namespace TicketFlow.Tests.UnitTests.Validators.Users
{
    public class UpdateUserDtoValidatorTests
    {
        private readonly UpdateUserDtoValidator _validator;

        public UpdateUserDtoValidatorTests()
        {
            _validator = new UpdateUserDtoValidator();
        }

        [Fact]
        public void Should_Not_Have_Error_When_All_Fields_Are_Null()
        {
            // Arrange
            var model = new UpdateUserDto
            {
                FullName = null,
                Email = null,
                Password = null
            };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Should_Have_Error_When_FullName_Is_Empty()
        {
            // Arrange
            var model = new UpdateUserDto
            {
                FullName = ""
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
            var model = new UpdateUserDto
            {
                FullName = "Sa"
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
            var model = new UpdateUserDto
            {
                FullName = new string('A', 101)
            };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.FullName);
        }

        [Fact]
        public void Should_Not_Have_Error_When_FullName_Is_Valid()
        {
            // Arrange
            var model = new UpdateUserDto
            {
                FullName = "Ferhat Samet Kalkan"
            };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.FullName);
        }

        [Fact]
        public void Should_Have_Error_When_Email_Is_Empty()
        {
            // Arrange
            var model = new UpdateUserDto
            {
                Email = ""
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
            var model = new UpdateUserDto
            {
                Email = "invalid-email"
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
            var model = new UpdateUserDto
            {
                Email = $"{new string('a', 92)}@test.com"
            };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Email_Is_Valid()
        {
            // Arrange
            var model = new UpdateUserDto
            {
                Email = "samet@test.com"
            };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Email);
        }

        [Fact]
        public void Should_Have_Error_When_Password_Is_Empty()
        {
            // Arrange
            var model = new UpdateUserDto
            {
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
            var model = new UpdateUserDto
            {
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
            var model = new UpdateUserDto
            {
                Password = new string('A', 101)
            };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Password);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Password_Is_Valid()
        {
            // Arrange
            var model = new UpdateUserDto
            {
                Password = "123456"
            };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Password);
        }

        [Fact]
        public void Should_Not_Have_Error_When_UpdateUserDto_Is_Valid()
        {
            // Arrange
            var model = new UpdateUserDto
            {
                FullName = "Ferhat Samet Kalkan",
                Email = "samet@test.com",
                Password = "123456"
            };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}