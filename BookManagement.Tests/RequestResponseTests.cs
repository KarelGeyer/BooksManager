using BookManagement.Common.Requests;
using BookManagement.Common.Responses;
using System.ComponentModel.DataAnnotations;

namespace BookManagement.Tests
{
    public class RequestResponseTests
    {
        [Fact]
        public void CreateBookRequest_ValidRequest_CreatedSuccessfully()
        {
            // Arrange & Act
            var request = new CreateBookRequest
            {
                Title = "Test Book",
                Author = "Test Author",
                ISBN = "1234567890",
                PublicationYear = 2024,
                AmountAvailable = 10
            };

            // Assert
            Assert.NotNull(request);
            Assert.Equal("Test Book", request.Title);
            Assert.Equal("Test Author", request.Author);
            Assert.Equal("1234567890", request.ISBN);
            Assert.Equal(2024, request.PublicationYear);
            Assert.Equal(10, request.AmountAvailable);
        }

        [Fact]
        public void CreateBookRequest_ValidateRequiredFields()
        {
            // Arrange
            var request = new CreateBookRequest
            {
                Title = string.Empty,
                Author = string.Empty,
                ISBN = "1234567890",
                PublicationYear = 2024,
                AmountAvailable = 10
            };

            var context = new ValidationContext(request);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(request, context, results, true);

            // Assert
            Assert.False(isValid);
            Assert.NotEmpty(results);
        }

        [Fact]
        public void BookResponse_CreatedSuccessfully()
        {
            // Arrange & Act
            var response = new BookResponse
            {
                Title = "Test Book",
                Author = "Test Author",
                ISBN = "1234567890",
                PublicationYear = 2024,
                AmountAvailable = 5
            };

            // Assert
            Assert.NotNull(response);
            Assert.Equal("Test Book", response.Title);
            Assert.Equal("Test Author", response.Author);
            Assert.Equal("1234567890", response.ISBN);
            Assert.Equal(2024, response.PublicationYear);
            Assert.Equal(5, response.AmountAvailable);
        }

        [Fact]
        public void LendResponse_CreatedSuccessfully()
        {
            // Arrange & Act
            var response = new LendResponse
            {
                NewAmount = 4,
                NameOfTheBook = "Test Book"
            };

            // Assert
            Assert.NotNull(response);
            Assert.Equal(4, response.NewAmount);
            Assert.Equal("Test Book", response.NameOfTheBook);
        }

        [Fact]
        public void CreateBookRequest_ISBNValidation()
        {
            // Arrange - ISBN must be 14-17 characters
            var validISBNs = new[] { "97812345678901", "978-1234567890" };
            var invalidISBNs = new[] { "123", "12345678901234567890", "1234567890" };

            foreach (var isbn in validISBNs)
            {
                // Act
                var request = new CreateBookRequest
                {
                    Title = "Book",
                    Author = "Author",
                    ISBN = isbn,
                    PublicationYear = 2024,
                    AmountAvailable = 5
                };

                var context = new ValidationContext(request);
                var results = new List<ValidationResult>();
                var isValid = Validator.TryValidateObject(request, context, results, true);

                // Assert
                Assert.True(isValid, $"ISBN {isbn} should be valid. Errors: {string.Join(", ", results.Select(r => r.ErrorMessage))}");
            }

            foreach (var isbn in invalidISBNs)
            {
                // Act
                var request = new CreateBookRequest
                {
                    Title = "Book",
                    Author = "Author",
                    ISBN = isbn,
                    PublicationYear = 2024,
                    AmountAvailable = 5
                };

                var context = new ValidationContext(request);
                var results = new List<ValidationResult>();
                var isValid = Validator.TryValidateObject(request, context, results, true);

                // Assert
                Assert.False(isValid, $"ISBN {isbn} should be invalid");
            }
        }
    }
}
