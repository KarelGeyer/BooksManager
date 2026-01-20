using BookManagement.Common.Requests;
using BookManagement.Common.Responses;
using BookManagement.Services.Interfaces;
using BookManagment.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace BookManagement.Tests
{
    public class BooksControllerTests
    {
        private readonly Mock<IBookService> _mockBookService;
        private readonly Mock<ILogger<BooksController>> _mockLogger;
        private readonly BooksController _controller;

        public BooksControllerTests()
        {
            _mockBookService = new Mock<IBookService>();
            _mockLogger = new Mock<ILogger<BooksController>>();
            _controller = new BooksController(_mockLogger.Object, _mockBookService.Object);
        }

        #region GetAll Tests

        [Fact]
        public async Task GetAll_ReturnsOkResult_WithBooks()
        {
            // Arrange
            var books = new List<BookResponse>
            {
                new BookResponse
                {
                    Title = "Book 1",
                    Author = "Author 1",
                    ISBN = "1234567890",
                    PublicationYear = 2020,
                    AmountAvailable = 5,
                },
                new BookResponse
                {
                    Title = "Book 2",
                    Author = "Author 2",
                    ISBN = "0987654321",
                    PublicationYear = 2021,
                    AmountAvailable = 3,
                },
            };

            _mockBookService
                .Setup(x => x.GetAll(It.IsAny<CancellationToken>()))
                .ReturnsAsync(books);

            // Act
            var result = await _controller.GetAll(CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedBooks = Assert.IsAssignableFrom<IEnumerable<BookResponse>>(okResult.Value);
            Assert.Equal(2, returnedBooks.Count());
        }

        [Fact]
        public async Task GetAll_ReturnsOkResult_WithEmptyList()
        {
            // Arrange
            var books = new List<BookResponse>();

            _mockBookService
                .Setup(x => x.GetAll(It.IsAny<CancellationToken>()))
                .ReturnsAsync(books);

            // Act
            var result = await _controller.GetAll(CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedBooks = Assert.IsAssignableFrom<IEnumerable<BookResponse>>(okResult.Value);
            Assert.Empty(returnedBooks);
        }

        #endregion

        #region GetByName Tests

        [Fact]
        public async Task GetByName_ReturnsOkResult_WhenBookFound()
        {
            // Arrange
            var name = "Test Book";
            var books = new List<BookResponse>
            {
                new BookResponse
                {
                    Title = name,
                    Author = "Author",
                    ISBN = "1234567890",
                    PublicationYear = 2020,
                    AmountAvailable = 5,
                },
            };

            _mockBookService
                .Setup(x => x.GetByName(It.IsAny<CancellationToken>(), name))
                .ReturnsAsync(books);

            // Act
            var result = await _controller.GetByName(name, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedBooks = Assert.IsAssignableFrom<IEnumerable<BookResponse>>(okResult.Value);
            Assert.Single(returnedBooks);
        }

        [Fact]
        public async Task GetByName_ReturnsOkResult_WhenNoBookFound()
        {
            // Arrange
            var books = new List<BookResponse>();

            _mockBookService
                .Setup(x => x.GetByName(It.IsAny<CancellationToken>(), It.IsAny<string>()))
                .ReturnsAsync(books);

            // Act
            var result = await _controller.GetByName("Nonexistent", CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedBooks = Assert.IsAssignableFrom<IEnumerable<BookResponse>>(okResult.Value);
            Assert.Empty(returnedBooks);
        }

        #endregion

        #region GetByAuthor Tests

        [Fact]
        public async Task GetByAuthor_ReturnsOkResult_WhenBooksFound()
        {
            // Arrange
            var author = "Test Author";
            var books = new List<BookResponse>
            {
                new BookResponse
                {
                    Title = "Book 1",
                    Author = author,
                    ISBN = "1234567890",
                    PublicationYear = 2020,
                    AmountAvailable = 5,
                },
                new BookResponse
                {
                    Title = "Book 2",
                    Author = author,
                    ISBN = "0987654321",
                    PublicationYear = 2021,
                    AmountAvailable = 3,
                },
            };

            _mockBookService
                .Setup(x => x.GetAllByAuthor(It.IsAny<CancellationToken>(), author))
                .ReturnsAsync(books);

            // Act
            var result = await _controller.GetByAuthor(author, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedBooks = Assert.IsAssignableFrom<IEnumerable<BookResponse>>(okResult.Value);
            Assert.Equal(2, returnedBooks.Count());
        }

        #endregion

        #region GetByISBN Tests

        [Fact]
        public async Task GetByISBN_ReturnsOkResult_WhenBookFound()
        {
            // Arrange
            var isbn = "1234567890";
            var book = new BookResponse
            {
                Title = "Test Book",
                Author = "Author",
                ISBN = isbn,
                PublicationYear = 2020,
                AmountAvailable = 5,
            };

            _mockBookService
                .Setup(x => x.GetByISBN(It.IsAny<CancellationToken>(), isbn))
                .ReturnsAsync(book);

            // Act
            var result = await _controller.GetByISBN(isbn, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedBook = Assert.IsType<BookResponse>(okResult.Value);
            Assert.Equal(isbn, returnedBook.ISBN);
        }

        [Fact]
        public async Task GetByISBN_ReturnsNotFound_WhenBookNotFound()
        {
            // Arrange
            _mockBookService
                .Setup(x => x.GetByISBN(It.IsAny<CancellationToken>(), It.IsAny<string>()))
                .ReturnsAsync((BookResponse)null);

            // Act
            var result = await _controller.GetByISBN("nonexistent", CancellationToken.None);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        #endregion

        #region CreateBook Tests

        [Fact]
        public async Task CreateBook_ReturnsCreatedAtActionResult_WhenSuccessful()
        {
            // Arrange
            var request = new CreateBookRequest
            {
                Title = "New Book",
                Author = "New Author",
                ISBN = "1111111111",
                PublicationYear = 2024,
                AmountAvailable = 10,
            };

            var response = new BookResponse
            {
                Title = request.Title,
                Author = request.Author,
                ISBN = request.ISBN,
                PublicationYear = request.PublicationYear,
                AmountAvailable = request.AmountAvailable,
            };

            _mockBookService
                .Setup(x => x.CreateBook(It.IsAny<CancellationToken>(), request))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.CreateBook(request, CancellationToken.None);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(nameof(BooksController.GetByISBN), createdResult.ActionName);
            Assert.IsType<BookResponse>(createdResult.Value);
        }

        [Fact]
        public async Task CreateBook_ThrowsException_WhenServiceFails()
        {
            // Arrange
            var request = new CreateBookRequest
            {
                Title = "Book",
                Author = "Author",
                ISBN = "1111111111",
                PublicationYear = 2024,
                AmountAvailable = 10,
            };

            _mockBookService
                .Setup(x => x.CreateBook(It.IsAny<CancellationToken>(), request))
                .ThrowsAsync(
                    new BookManagement.Common.Exceptions.FailedToCreateException(typeof(object))
                );

            // Act & Assert
            await Assert.ThrowsAsync<BookManagement.Common.Exceptions.FailedToCreateException>(
                () => _controller.CreateBook(request, CancellationToken.None)
            );
        }

        #endregion

        #region LendBook Tests

        [Fact]
        public async Task LendBook_ReturnsOkResult_WhenSuccessful()
        {
            // Arrange
            var isbn = "1234567890";
            var response = new LendResponse { NewAmount = 4, NameOfTheBook = "Test Book" };

            _mockBookService
                .Setup(x => x.DecreaseAvailableBookAmount(It.IsAny<CancellationToken>(), isbn))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.LendBook(isbn, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedResponse = Assert.IsType<LendResponse>(okResult.Value);
            Assert.Equal(4, returnedResponse.NewAmount);
        }

        [Fact]
        public async Task LendBook_ThrowsException_WhenBookNotAvailable()
        {
            // Arrange
            var isbn = "1234567890";

            _mockBookService
                .Setup(x => x.DecreaseAvailableBookAmount(It.IsAny<CancellationToken>(), isbn))
                .ThrowsAsync(
                    new BookManagement.Common.Exceptions.FailedToLendException(typeof(object))
                );

            // Act & Assert
            await Assert.ThrowsAsync<BookManagement.Common.Exceptions.FailedToLendException>(
                () => _controller.LendBook(isbn, CancellationToken.None)
            );
        }

        #endregion

        #region ReturnBook Tests

        [Fact]
        public async Task ReturnBook_ReturnsOkResult_WhenSuccessful()
        {
            // Arrange
            var isbn = "1234567890";
            var response = new LendResponse { NewAmount = 6, NameOfTheBook = "Test Book" };

            _mockBookService
                .Setup(x => x.IncreaseAvailableBookAmount(It.IsAny<CancellationToken>(), isbn))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.ReturnBook(isbn, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedResponse = Assert.IsType<LendResponse>(okResult.Value);
            Assert.Equal(6, returnedResponse.NewAmount);
        }

        [Fact]
        public async Task ReturnBook_ThrowsException_WhenBookNotFound()
        {
            // Arrange
            var isbn = "nonexistent";

            _mockBookService
                .Setup(x => x.IncreaseAvailableBookAmount(It.IsAny<CancellationToken>(), isbn))
                .ThrowsAsync(
                    new BookManagement.Common.Exceptions.NotFoundException(typeof(object))
                );

            // Act & Assert
            await Assert.ThrowsAsync<BookManagement.Common.Exceptions.NotFoundException>(
                () => _controller.ReturnBook(isbn, CancellationToken.None)
            );
        }

        #endregion
    }
}
