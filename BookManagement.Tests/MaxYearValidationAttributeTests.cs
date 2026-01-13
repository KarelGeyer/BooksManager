using BookManagement.Common.Attributes;
using System.ComponentModel.DataAnnotations;

namespace BookManagement.Tests
{
    public class MaxYearValidationAttributeTests
    {
        [Fact]
        public void IsValid_ReturnSuccess_WhenYearIsCurrentYear()
        {
            // Arrange
            var attribute = new MaxYearValidationAttribute();
            var currentYear = (short)DateTime.Now.Year;
            var context = new ValidationContext(currentYear);

            // Act
            var result = attribute.GetValidationResult(currentYear, context);

            // Assert
            Assert.Equal(ValidationResult.Success, result);
        }

        [Fact]
        public void IsValid_ReturnSuccess_WhenYearIsPast()
        {
            // Arrange
            var attribute = new MaxYearValidationAttribute();
            var pastYear = (short)(DateTime.Now.Year - 10);
            var context = new ValidationContext(pastYear);

            // Act
            var result = attribute.GetValidationResult(pastYear, context);

            // Assert
            Assert.Equal(ValidationResult.Success, result);
        }

        [Fact]
        public void IsValid_ReturnError_WhenYearIsInFuture()
        {
            // Arrange
            var attribute = new MaxYearValidationAttribute();
            var futureYear = (short)(DateTime.Now.Year + 2);
            var context = new ValidationContext(futureYear);

            // Act
            var result = attribute.GetValidationResult(futureYear, context);

            // Assert
            Assert.NotEqual(ValidationResult.Success, result);
            Assert.NotNull(result);
        }

        [Fact]
        public void IsValid_ReturnSuccess_WhenYearIsZero()
        {
            // Arrange
            var attribute = new MaxYearValidationAttribute();
            var context = new ValidationContext((short)0);

            // Act
            var result = attribute.GetValidationResult((short)0, context);

            // Assert
            Assert.Equal(ValidationResult.Success, result);
        }

        [Fact]
        public void IsValid_ReturnError_WhenYearIsMuchInFuture()
        {
            // Arrange
            var attribute = new MaxYearValidationAttribute();
            var context = new ValidationContext((short)9999);

            // Act
            var result = attribute.GetValidationResult((short)9999, context);

            // Assert
            Assert.NotEqual(ValidationResult.Success, result);
            Assert.NotNull(result);
        }
    }
}

