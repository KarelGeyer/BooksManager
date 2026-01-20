using BookManagement.Common.Entities;
using BookManagement.Common.Exceptions;

namespace BookManagement.Tests
{
    public class ExceptionTests
    {
        [Fact]
        public void FailedToCreateException_CreatesException_WithTypeAndMessage()
        {
            // Arrange
            var type = typeof(Book);
            var message = "Test error message";

            // Act
            var exception = new FailedToCreateException(type, message);

            // Assert
            Assert.NotNull(exception);
            Assert.Contains(type.Name, exception.Message);
            Assert.Contains(message, exception.Message);
        }

        [Fact]
        public void FailedToCreateException_CreatesException_WithTypeAndInnerException()
        {
            // Arrange
            var type = typeof(Book);
            var innerException = new InvalidOperationException("Inner error");

            // Act
            var exception = new FailedToCreateException(type, "Test error", innerException);

            // Assert
            Assert.NotNull(exception);
            Assert.Equal(innerException, exception.InnerException);
        }

        [Fact]
        public void NotFoundException_CreatesException_WithType()
        {
            // Arrange
            var type = typeof(Book);

            // Act
            var exception = new NotFoundException(type);

            // Assert
            Assert.NotNull(exception);
            Assert.Contains(type.Name, exception.Message);
        }

        [Fact]
        public void NotFoundException_CreatesException_WithTypeAndIdentifier()
        {
            // Arrange
            var type = typeof(Book);
            var identifier = "1234567890";

            // Act
            var exception = new NotFoundException(type, identifier);

            // Assert
            Assert.NotNull(exception);
            Assert.Contains(type.Name, exception.Message);
            Assert.Contains(identifier, exception.Message);
        }

        [Fact]
        public void FailedToLendException_CreatesException_WithType()
        {
            // Arrange
            var type = typeof(Book);

            // Act
            var exception = new FailedToLendException(type);

            // Assert
            Assert.NotNull(exception);
            Assert.Contains(type.Name, exception.Message);
        }

        [Fact]
        public void FailedToLendException_CreatesException_WithTypeAndMessage()
        {
            // Arrange
            var type = typeof(Book);
            var message = "Book not available";

            // Act
            var exception = new FailedToLendException(type, message);

            // Assert
            Assert.NotNull(exception);
            Assert.Contains(type.Name, exception.Message);
            Assert.Contains(message, exception.Message);
        }
    }
}
