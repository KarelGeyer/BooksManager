using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Xml.Linq;
using BookManagement.Common.Entities;
using BookManagement.Common.Exceptions;
using BookManagement.Common.Requests;
using BookManagement.Common.Responses;
using BookManagement.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BookManagment.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly ILogger<BooksController> _logger;
        private readonly IBookService _bookService;

        public BooksController(ILogger<BooksController> logger, IBookService bookService)
        {
            _logger = logger;
            _bookService = bookService;
        }

        /// <summary>
        /// Retrieves all books.
        /// </summary>
        /// <param name="ct">
        /// Cancellation token used to cancel the request.
        /// </param>
        /// <returns>
        /// An <see cref="ActionResult"/> containing a collection of <see cref="BookResponse"/> objects.
        /// </returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookResponse>>> GetAll(CancellationToken ct)
        {
            _logger.LogInformation("Fetching all books");
            var books = await _bookService.GetAll(ct);
            return Ok(books);
        }

        /// <summary>
        /// Retrieves books that match the specified name.
        /// </summary>
        /// <param name="name">
        /// The book name or partial name to search for. Must be at least two characters long.
        /// </param>
        /// <param name="ct">
        /// Cancellation token used to cancel the request.
        /// </param>
        /// <returns>
        /// An <see cref="ActionResult"/> containing a collection of <see cref="BookResponse"/> objects.
        /// </returns>
        [HttpGet("name/{name}")]
        public async Task<ActionResult<IEnumerable<BookResponse>>> GetByName(
            [Required] [MinLength(2)] string name,
            CancellationToken ct
        )
        {
            _logger.LogInformation("Searching books by name: {BookName}", name);
            var books = await _bookService.GetByName(ct, name);
            return Ok(books);
        }

        /// <summary>
        /// Retrieves books written by the specified author.
        /// </summary>
        /// <param name="author">
        /// The author name or partial name to search for. Must be at least two characters long.
        /// </param>
        /// <param name="ct">
        /// Cancellation token used to cancel the request.
        /// </param>
        /// <returns>
        /// An <see cref="ActionResult"/> containing a collection of <see cref="BookResponse"/> objects.
        /// </returns>
        [HttpGet("author/{author}")]
        public async Task<ActionResult<IEnumerable<BookResponse>>> GetByAuthor(
            [Required] [MinLength(2)] string author,
            CancellationToken ct
        )
        {
            _logger.LogInformation("Searching books by author: {BookName}", author);
            var books = await _bookService.GetAllByAuthor(ct, author);
            return Ok(books);
        }

        /// <summary>
        /// Retrieves a book by its ISBN.
        /// </summary>
        /// <param name="isbn">
        /// The International Standard Book Number (ISBN) of the book.
        /// Must be between 10 and 17 characters long.
        /// </param>
        /// <param name="ct">
        /// Cancellation token used to cancel the request.
        /// </param>
        /// <returns>
        /// An <see cref="ActionResult"/> containing the requested <see cref="BookResponse"/>.
        /// </returns>
        [HttpGet("isbn/{isbn}")]
        public async Task<ActionResult<BookResponse>> GetByISBN(
            [Required]
            [StringLength(
                17,
                MinimumLength = 14,
                ErrorMessage = "ISBN must have 10 to 13 characters."
            )]
                string isbn,
            CancellationToken ct
        )
        {
            _logger.LogInformation("Searching book by ISBN: {Isbn}", isbn);
            var book = await _bookService.GetByISBN(ct, isbn);

            if (book == null)
            {
                _logger.LogWarning("Book with ISBN {Isbn} was not found.", isbn);
                return NotFound(new { error = $"Book with ISBN '{isbn}' was not found." });
            }

            return Ok(book);
        }

        /// <summary>
        /// Creates a new book.
        /// </summary>
        /// <param name="model">
        /// The data required to create a new book, including ISBN, title, author,
        /// publication year, and available amount.
        /// </param>
        /// <param name="ct">
        /// Cancellation token used to cancel the request.
        /// </param>
        /// <returns>
        /// An <see cref="ActionResult"/> containing the newly created <see cref="BookResponse"/>.
        /// </returns>
        /// <remarks>
        /// Validation rules:
        /// - ISBN must be between 14 and 17 characters
        /// - Title is required and limited to 200 characters
        /// - Author is required and limited to 100 characters
        /// - Publication year must be greater than or equal to 1900
        /// - Amount available must be zero or greater
        /// </remarks>
        [HttpPost("create")]
        public async Task<ActionResult<BookResponse>> CreateBook(
            [FromBody] CreateBookRequest model,
            CancellationToken ct
        )
        {
            _logger.LogInformation("Creating a new book with ISBN: {Isbn}", model.ISBN);
            var createdBook = await _bookService.CreateBook(ct, model);
            return CreatedAtAction(nameof(GetByISBN), new { isbn = createdBook.ISBN }, createdBook);
        }

        /// <summary>
        /// Lends a book by decreasing its available amount.
        /// </summary>
        /// <param name="isbn">
        /// The International Standard Book Number (ISBN) of the book to be lent.
        /// Must be between 10 and 17 characters long.
        /// </param>
        /// <param name="ct">
        /// Cancellation token used to cancel the request.
        /// </param>
        /// <returns>
        /// An <see cref="ActionResult"/> containing the <see cref="LendResponse"/> result.
        /// </returns>
        [HttpPost("lend/{isbn}")]
        public async Task<ActionResult<LendResponse>> LendBook(
            [Required]
            [StringLength(
                17,
                MinimumLength = 14,
                ErrorMessage = "ISBN must have 10 to 13 characters."
            )]
                string isbn,
            CancellationToken ct
        )
        {
            _logger.LogInformation("Request to lend a book: {ISBN}", isbn);
            var response = await _bookService.DecreaseAvailableBookAmount(ct, isbn);
            return Ok(response);
        }

        /// <summary>
        /// Returns a previously lent book by increasing its available amount.
        /// </summary>
        /// <param name="isbn">
        /// The International Standard Book Number (ISBN) of the book to be returned.
        ///— Must be between 10 and 17 characters long.
        /// </param>
        /// <param name="ct">
        /// Cancellation token used to cancel the request.
        /// </param>
        /// <returns>
        /// An <see cref="ActionResult"/> containing the <see cref="LendResponse"/> result.
        /// </returns>
        [HttpPost("return/{isbn}")]
        public async Task<ActionResult<LendResponse>> ReturnBook(
            [Required]
            [StringLength(
                17,
                MinimumLength = 14,
                ErrorMessage = "ISBN must have 10 to 13 characters."
            )]
                string isbn,
            CancellationToken ct
        )
        {
            _logger.LogInformation("Request to return a book: {ISBN}", isbn);
            var response = await _bookService.IncreaseAvailableBookAmount(ct, isbn);
            return Ok(response);
        }
    }
}
