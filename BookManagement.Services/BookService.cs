using BookManagement.Common.Entities;
using BookManagement.Common.Exceptions;
using BookManagement.Common.Requests;
using BookManagement.Common.Responses;
using BookManagement.DbService.Interfaces;
using BookManagement.Services.Interfaces;

namespace BookManagement.Services
{
    public class BookService : IBookService
    {
        private readonly IDbService<Book> _dbService;

        public BookService(IDbService<Book> dbService)
        {
            _dbService = dbService;
        }

        /// <inheritdoc />
        public async Task<BookResponse?> GetByISBN(CancellationToken ct, string isbn)
        {
            var book = await _dbService.GetAsync(ct, x => x.ISBN == isbn);

            if (book == null)
                return null;

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
        public async Task<BookResponse> CreateBook(CancellationToken ct, CreateBookRequest model)
        {
            var existingBook = await _dbService.GetAsync(ct, x => x.ISBN == model.ISBN);
            if (existingBook != null)
            {
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

            var result = await _dbService.CreateAsync(ct, newBook);

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
                await _dbService.GetAsync(ct, x => x.ISBN == isbn)
                ?? throw new NotFoundException(typeof(Book), isbn);

            if (book.AmountAvailable <= 0)
            {
                throw new FailedToLendExpection(typeof(Book), "book is not available for lending");
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
                await _dbService.GetAsync(ct, x => x.ISBN == isbn)
                ?? throw new NotFoundException(typeof(Book), isbn);
            book.AmountAvailable++;
            return await HandleBookLending(ct, book);
        }

        /// <summary>
        /// Handle the lending and returning of books
        /// </summary>
        /// <param name="book">updated <see cref="Book"/></param>
        /// <returns>An instance of <see cref=""/></returns>
        /// <exception cref="FailedToLendExpection"></exception>
        private async Task<LendResponse> HandleBookLending(CancellationToken ct, Book book)
        {
            try
            {
                var result = await _dbService.UpdateAsync(ct, b => b.ISBN == book.ISBN, book);

                if (result == null)
                {
                    throw new FailedToLendExpection(typeof(Book));
                }

                return new LendResponse()
                {
                    NewAmount = book.AmountAvailable,
                    NameOfTheBook = book.Title ?? string.Empty,
                };
            }
            catch (Exception ex)
            {
                throw new FailedToLendExpection(typeof(Book), ex.Message);
            }
        }
    }
}
