using System.Xml.Linq;
using BookManagement.Common.Entities;
using BookManagement.Common.Exceptions;
using BookManagement.Common.Requests;
using BookManagement.Common.Responses;
using BookManagement.DbService.Interfaces;
using BookManagement.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace BookManagement.Services
{
    public class BookService : IBookService
    {
        private readonly IDbService<Book> _dbService;
        private readonly ILogger<BookService> _logger;

        public BookService(ILogger<BookService> logger, IDbService<Book> dbService)
        {
            _logger = logger;
            _dbService = dbService;
        }

        /// <inheritdoc />
        public async Task<BookResponse?> GetByISBN(CancellationToken ct, string isbn)
        {
            var book = await _dbService.GetAsync(x => x.ISBN == isbn, ct);

            if (book == null)
            {
                _logger.LogInformation("Book not found");
                return null;
            }

            return new BookResponse()
            {
                Title = book.Title ?? "Unknown",
                Author = book.Author ?? "Unknown",
                ISBN = book.ISBN ?? string.Empty,
                PublicationYear = book.PublicationYear,
                AmountAvailable = book.AmountAvailable,
            };
        }

        /// <inheritdoc />
        public async Task<List<BookResponse>> GetByName(CancellationToken ct, string name)
        {
            var books = await _dbService.GetAllAsync(ct, x => x.Title == name);

            if (books.Count == 0)
                _logger.LogInformation($"Books by name {name} not found");

            return books
                .Select(book => new BookResponse
                {
                    Title = book.Title ?? "Unknown",
                    Author = book.Author ?? "Unknown",
                    ISBN = book.ISBN ?? string.Empty,
                    PublicationYear = book.PublicationYear,
                    AmountAvailable = book.AmountAvailable,
                })
                .ToList();
        }

        /// <inheritdoc />
        public async Task<List<BookResponse>> GetAllByAuthor(CancellationToken ct, string author)
        {
            var books = await _dbService.GetAllAsync(ct, x => x.Author == author);

            if (books.Count == 0)
                _logger.LogInformation($"Books by author {author} not found");

            return books
                .Select(book => new BookResponse
                {
                    Title = book.Title ?? "Unknown",
                    Author = book.Author ?? "Unknown",
                    ISBN = book.ISBN ?? string.Empty,
                    PublicationYear = book.PublicationYear,
                    AmountAvailable = book.AmountAvailable,
                })
                .ToList();
        }

        /// <inheritdoc />
        public async Task<List<BookResponse>> GetAll(CancellationToken ct)
        {
            var books = await _dbService.GetAllAsync(ct);

            if (books.Count == 0)
                _logger.LogCritical($"No books found");

            return books
                .Select(book => new BookResponse
                {
                    Title = book.Title ?? "Unknown",
                    Author = book.Author ?? "Unknown",
                    ISBN = book.ISBN ?? string.Empty,
                    PublicationYear = book.PublicationYear,
                    AmountAvailable = book.AmountAvailable,
                })
                .ToList();
        }

        /// <inheritdoc />
        public async Task<PagedResult<BookResponse>> GetAll(
            CancellationToken ct,
            int page = 1,
            int pageSize = 10
        )
        {
            int skip = (page - 1) * pageSize;
            var books = await _dbService.GetAllAsync(
                ct,
                predicate: null,
                orderBy: x => x.Id,
                skip: skip,
                take: pageSize
            );

            var response = books
                .Select(book => new BookResponse
                {
                    Title = book.Title ?? "Unknown",
                    Author = book.Author ?? "Unknown",
                    ISBN = book.ISBN ?? string.Empty,
                    PublicationYear = book.PublicationYear,
                    AmountAvailable = book.AmountAvailable,
                })
                .ToList();

            return new()
            {
                Items = response,
                TotalCount = response.Count,
                Page = page,
                PageSize = pageSize,
            };
        }

        /// <inheritdoc />
        public async Task<BookResponse> CreateBook(CancellationToken ct, CreateBookRequest model)
        {
            var existingBook = await _dbService.GetAsync(x => x.ISBN == model.ISBN, ct);
            if (existingBook != null)
            {
                _logger.LogError($"A book with ISBN '{model.ISBN}' already exists.");
                throw new FailedToCreateException(
                    typeof(Book),
                    $"A book with ISBN '{model.ISBN}' already exists."
                );
            }

            var newBook = new Book
            {
                Author = model.Author,
                Title = model.Title,
                PublicationYear = model.PublicationYear,
                ISBN = model.ISBN,
                AmountAvailable = model.AmountAvailable,
            };

            var result = await _dbService.CreateAsync(newBook, ct);
            _logger.LogInformation($"A book with ISBN '{model.ISBN}' succesfully created.");

            return new BookResponse()
            {
                Title = result.Title ?? "Unknown",
                Author = result.Author ?? "Unknown",
                ISBN = result.ISBN ?? string.Empty,
                PublicationYear = result.PublicationYear,
                AmountAvailable = result.AmountAvailable,
            };
        }

        /// <inheritdoc />
        public async Task<LendResponse> DecreaseAvailableBookAmount(
            CancellationToken ct,
            string isbn
        )
        {
            var book =
                await _dbService.GetAsync(x => x.ISBN == isbn, ct)
                ?? throw new NotFoundException(typeof(Book), isbn);

            if (book.AmountAvailable <= 0)
            {
                _logger.LogError($"A book with ISBN '{isbn}'is not available for lending.");
                throw new FailedToLendException(typeof(Book), "book is not available for lending");
            }

            book.AmountAvailable--;
            return await HandleBookLending(ct, book);
        }

        /// <inheritdoc />
        public async Task<LendResponse> IncreaseAvailableBookAmount(
            CancellationToken ct,
            string isbn
        )
        {
            var book =
                await _dbService.GetAsync(x => x.ISBN == isbn, ct)
                ?? throw new NotFoundException(typeof(Book), isbn);
            book.AmountAvailable++;
            return await HandleBookLending(ct, book);
        }

        /// <summary>
        /// Handle the lending and returning of books
        /// </summary>
        /// <param name="book">updated <see cref="Book"/></param>
        /// <returns>An instance of <see cref=""/></returns>
        /// <exception cref="FailedToLendException"></exception>
        private async Task<LendResponse> HandleBookLending(CancellationToken ct, Book book)
        {
            try
            {
                var result = await _dbService.UpdateAsync(b => b.ISBN == book.ISBN, book, ct);

                if (result == null)
                {
                    _logger.LogError($"Failed to update a storage of a book with isbn {book.ISBN}");
                    throw new FailedToLendException(typeof(Book));
                }

                _logger.LogInformation($"Book with isbn {book.ISBN} successfuly updated");
                return new LendResponse()
                {
                    NewAmount = book.AmountAvailable,
                    NameOfTheBook = book.Title ?? string.Empty,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    $"An error occured when attempting to lend or return book with isnb {book.ISBN}"
                );
                throw new FailedToLendException(typeof(Book), ex.Message);
            }
        }
    }
}
