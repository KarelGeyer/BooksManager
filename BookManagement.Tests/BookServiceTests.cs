using BookManagement.Common.Entities;
using BookManagement.Common.Requests;
using BookManagement.Common.Responses;
using BookManagement.DbService.Interfaces;
using BookManagement.Services;
using Moq;

namespace BookManagement.Tests
{
    public class BookServiceTests
    {
        private readonly Mock<IDbService<Book>> _mockDbService;
        private readonly BookService _bookService;

        public BookServiceTests()
        {
            _mockDbService = new Mock<IDbService<Book>>();
            _bookService = new BookService(_mockDbService.Object);
        }

        #region GetAll Tests

        [Fact]
        public async Task GetAll_ReturnsAllBooks_WhenBooksExist()
        {
            // Arrange
            var books = new List<Book>
            {
                new Book
                {
                    Id = 1,
                    Title = "Book 1",
                    Author = "Author 1",
                    ISBN = "1234567890",
                    PublicationYear = 2020,
                    AmountAvailable = 5,
                },
                new Book
                {
                    Id = 2,
                    Title = "Book 2",
                    Author = "Author 2",
                    ISBN = "0987654321",
                    PublicationYear = 2021,
                    AmountAvailable = 3,
                },
            };

            _mockDbService
                .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>(), null))
                .ReturnsAsync(books);

            // Act
            var result = await _bookService.GetAll(CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("Book 1", result[0].Title);
            Assert.Equal("Author 2", result[1].Author);
            _mockDbService.Verify(
                x => x.GetAllAsync(It.IsAny<CancellationToken>(), null),
                Times.Once
            );
        }

        [Fact]
        public async Task GetAll_ReturnsEmptyList_WhenNoBooksExist()
        {
            // Arrange
            _mockDbService
                .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>(), null))
                .ReturnsAsync(new List<Book>());

            // Act
            var result = await _bookService.GetAll(CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        #endregion

        #region GetByISBN Tests

        [Fact]
        public async Task GetByISBN_ReturnsBook_WhenBookExists()
        {
            // Arrange
            var isbn = "1234567890";
            var book = new Book
            {
                Id = 1,
                Title = "Test Book",
                Author = "Test Author",
                ISBN = isbn,
                PublicationYear = 2020,
                AmountAvailable = 5,
            };

            _mockDbService
                .Setup(x =>
                    x.GetAsync(
                        It.IsAny<CancellationToken>(),
                        It.IsAny<System.Linq.Expressions.Expression<System.Func<Book, bool>>>()
                    )
                )
                .ReturnsAsync(book);

            // Act
            var result = await _bookService.GetByISBN(CancellationToken.None, isbn);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test Book", result.Title);
            Assert.Equal("Test Author", result.Author);
            Assert.Equal(isbn, result.ISBN);
        }

        [Fact]
        public async Task GetByISBN_ReturnsNull_WhenBookDoesNotExist()
        {
            // Arrange
            _mockDbService
                .Setup(x =>
                    x.GetAsync(
                        It.IsAny<CancellationToken>(),
                        It.IsAny<System.Linq.Expressions.Expression<System.Func<Book, bool>>>()
                    )
                )
                .ReturnsAsync((Book)null);

            // Act
            var result = await _bookService.GetByISBN(CancellationToken.None, "nonexistent");

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region GetByName Tests

        [Fact]
        public async Task GetByName_ReturnsBooksByName_WhenBooksExist()
        {
            // Arrange
            var name = "Test Book";
            var books = new List<Book>
            {
                new Book
                {
                    Id = 1,
                    Title = name,
                    Author = "Author 1",
                    ISBN = "1234567890",
                    PublicationYear = 2020,
                    AmountAvailable = 5,
                },
            };

            _mockDbService
                .Setup(x =>
                    x.GetAllAsync(
                        It.IsAny<CancellationToken>(),
                        It.IsAny<System.Linq.Expressions.Expression<System.Func<Book, bool>>>()
                    )
                )
                .ReturnsAsync(books);

            // Act
            var result = await _bookService.GetByName(CancellationToken.None, name);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(name, result[0].Title);
        }

        [Fact]
        public async Task GetByName_ReturnsEmptyList_WhenNoMatchFound()
        {
            // Arrange
            _mockDbService
                .Setup(x =>
                    x.GetAllAsync(
                        It.IsAny<CancellationToken>(),
                        It.IsAny<System.Linq.Expressions.Expression<System.Func<Book, bool>>>()
                    )
                )
                .ReturnsAsync(new List<Book>());

            // Act
            var result = await _bookService.GetByName(CancellationToken.None, "Nonexistent");

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        #endregion

        #region GetAllByAuthor Tests

        [Fact]
        public async Task GetAllByAuthor_ReturnsBooksbyAuthor_WhenBooksExist()
        {
            // Arrange
            var author = "Test Author";
            var books = new List<Book>
            {
                new Book
                {
                    Id = 1,
                    Title = "Book 1",
                    Author = author,
                    ISBN = "1234567890",
                    PublicationYear = 2020,
                    AmountAvailable = 5,
                },
                new Book
                {
                    Id = 2,
                    Title = "Book 2",
                    Author = author,
                    ISBN = "0987654321",
                    PublicationYear = 2021,
                    AmountAvailable = 3,
                },
            };

            _mockDbService
                .Setup(x =>
                    x.GetAllAsync(
                        It.IsAny<CancellationToken>(),
                        It.IsAny<System.Linq.Expressions.Expression<System.Func<Book, bool>>>()
                    )
                )
                .ReturnsAsync(books);

            // Act
            var result = await _bookService.GetAllByAuthor(CancellationToken.None, author);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.All(result, book => Assert.Equal(author, book.Author));
        }

        #endregion

        #region CreateBook Tests

        [Fact]
        public async Task CreateBook_CreatesNewBook_WhenISBNIsUnique()
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

            _mockDbService
                .Setup(x =>
                    x.GetAsync(
                        It.IsAny<CancellationToken>(),
                        It.IsAny<System.Linq.Expressions.Expression<System.Func<Book, bool>>>()
                    )
                )
                .ReturnsAsync((Book)null);

            var createdBook = new Book
            {
                Id = 1,
                Title = request.Title,
                Author = request.Author,
                ISBN = request.ISBN,
                PublicationYear = request.PublicationYear,
                AmountAvailable = request.AmountAvailable,
            };

            _mockDbService
                .Setup(x => x.CreateAsync(It.IsAny<CancellationToken>(), It.IsAny<Book>()))
                .ReturnsAsync(createdBook);

            // Act
            var result = await _bookService.CreateBook(CancellationToken.None, request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(request.Title, result.Title);
            Assert.Equal(request.ISBN, result.ISBN);
            _mockDbService.Verify(
                x => x.CreateAsync(It.IsAny<CancellationToken>(), It.IsAny<Book>()),
                Times.Once
            );
        }

        [Fact]
        public async Task CreateBook_ThrowsException_WhenISBNAlreadyExists()
        {
            // Arrange
            var request = new CreateBookRequest
            {
                Title = "Duplicate Book",
                Author = "Author",
                ISBN = "1111111111",
                PublicationYear = 2024,
                AmountAvailable = 10,
            };

            var existingBook = new Book { ISBN = request.ISBN };

            _mockDbService
                .Setup(x =>
                    x.GetAsync(
                        It.IsAny<CancellationToken>(),
                        It.IsAny<System.Linq.Expressions.Expression<System.Func<Book, bool>>>()
                    )
                )
                .ReturnsAsync(existingBook);

            // Act & Assert
            await Assert.ThrowsAsync<BookManagement.Common.Exceptions.FailedToCreateException>(
                () => _bookService.CreateBook(CancellationToken.None, request)
            );
        }

        #endregion

        #region DecreaseAvailableBookAmount Tests

        [Fact]
        public async Task DecreaseAvailableBookAmount_DecreasesAmount_WhenBooksAvailable()
        {
            // Arrange
            var isbn = "1234567890";
            var book = new Book
            {
                Id = 1,
                Title = "Test Book",
                Author = "Test Author",
                ISBN = isbn,
                PublicationYear = 2020,
                AmountAvailable = 5,
            };

            _mockDbService
                .Setup(x =>
                    x.GetAsync(
                        It.IsAny<CancellationToken>(),
                        It.IsAny<System.Linq.Expressions.Expression<System.Func<Book, bool>>>()
                    )
                )
                .ReturnsAsync(book);

            var expectedBook = new Book
            {
                Id = 1,
                Title = "Test Book",
                Author = "Test Author",
                ISBN = isbn,
                PublicationYear = 2020,
                AmountAvailable = 4,
            };

            _mockDbService
                .Setup(x =>
                    x.UpdateAsync(
                        It.IsAny<CancellationToken>(),
                        It.IsAny<System.Linq.Expressions.Expression<System.Func<Book, bool>>>(),
                        It.IsAny<Book>()
                    )
                )
                .ReturnsAsync(expectedBook);

            // Act
            var result = await _bookService.DecreaseAvailableBookAmount(
                CancellationToken.None,
                isbn
            );

            // Assert
            Assert.NotNull(result);
            Assert.Equal(4, result.NewAmount);
            Assert.Equal("Test Book", result.NameOfTheBook);
        }

        [Fact]
        public async Task DecreaseAvailableBookAmount_ThrowsException_WhenBookNotFound()
        {
            // Arrange
            _mockDbService
                .Setup(x =>
                    x.GetAsync(
                        It.IsAny<CancellationToken>(),
                        It.IsAny<System.Linq.Expressions.Expression<System.Func<Book, bool>>>()
                    )
                )
                .ReturnsAsync((Book)null);

            // Act & Assert
            await Assert.ThrowsAsync<BookManagement.Common.Exceptions.NotFoundException>(
                () =>
                    _bookService.DecreaseAvailableBookAmount(CancellationToken.None, "nonexistent")
            );
        }

        [Fact]
        public async Task DecreaseAvailableBookAmount_ThrowsException_WhenNoBooksAvailable()
        {
            // Arrange
            var isbn = "1234567890";
            var book = new Book
            {
                Id = 1,
                Title = "Test Book",
                Author = "Test Author",
                ISBN = isbn,
                PublicationYear = 2020,
                AmountAvailable = 0,
            };

            _mockDbService
                .Setup(x =>
                    x.GetAsync(
                        It.IsAny<CancellationToken>(),
                        It.IsAny<System.Linq.Expressions.Expression<System.Func<Book, bool>>>()
                    )
                )
                .ReturnsAsync(book);

            // Act & Assert
            await Assert.ThrowsAsync<BookManagement.Common.Exceptions.FailedToLendException>(
                () => _bookService.DecreaseAvailableBookAmount(CancellationToken.None, isbn)
            );
        }

        #endregion

        #region IncreaseAvailableBookAmount Tests

        [Fact]
        public async Task IncreaseAvailableBookAmount_IncreasesAmount_WhenBookExists()
        {
            // Arrange
            var isbn = "1234567890";
            var book = new Book
            {
                Id = 1,
                Title = "Test Book",
                Author = "Test Author",
                ISBN = isbn,
                PublicationYear = 2020,
                AmountAvailable = 5,
            };

            _mockDbService
                .Setup(x =>
                    x.GetAsync(
                        It.IsAny<CancellationToken>(),
                        It.IsAny<System.Linq.Expressions.Expression<System.Func<Book, bool>>>()
                    )
                )
                .ReturnsAsync(book);

            var expectedBook = new Book
            {
                Id = 1,
                Title = "Test Book",
                Author = "Test Author",
                ISBN = isbn,
                PublicationYear = 2020,
                AmountAvailable = 6,
            };

            _mockDbService
                .Setup(x =>
                    x.UpdateAsync(
                        It.IsAny<CancellationToken>(),
                        It.IsAny<System.Linq.Expressions.Expression<System.Func<Book, bool>>>(),
                        It.IsAny<Book>()
                    )
                )
                .ReturnsAsync(expectedBook);

            // Act
            var result = await _bookService.IncreaseAvailableBookAmount(
                CancellationToken.None,
                isbn
            );

            // Assert
            Assert.NotNull(result);
            Assert.Equal(6, result.NewAmount);
        }

        [Fact]
        public async Task IncreaseAvailableBookAmount_ThrowsException_WhenBookNotFound()
        {
            // Arrange
            _mockDbService
                .Setup(x =>
                    x.GetAsync(
                        It.IsAny<CancellationToken>(),
                        It.IsAny<System.Linq.Expressions.Expression<System.Func<Book, bool>>>()
                    )
                )
                .ReturnsAsync((Book)null);

            // Act & Assert
            await Assert.ThrowsAsync<BookManagement.Common.Exceptions.NotFoundException>(
                () =>
                    _bookService.IncreaseAvailableBookAmount(CancellationToken.None, "nonexistent")
            );
        }

        #endregion
    }
}
