using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using BookManagement.Common.Entities;
using BookManagement.Common.Responses;
using BookManagement.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Book_managmenet.Controllers
{
    [Route("api/v1/")]
    [ApiController]
    [Produces("application/json")]
    public class ExternalController : ControllerBase
    {
        private ILogger<ExternalController> _logger;
        private readonly IBookService _bookService;

        public ExternalController(ILogger<ExternalController> logger, IBookService bookService)
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
        [HttpGet("get")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Book>))]
        public async Task<ActionResult<BookResponse>> GetAll(CancellationToken ct)
        {
            _logger.LogInformation("Fetching all books from the database.");
            var books = await _bookService.GetAll(ct);

            _logger.LogDebug("Successfully retrieved {Count} books.", books.Count());
            return Ok(books);
        }

        /// <summary>
        /// Retrieves all books by size.
        /// </summary>
        /// <param name="ct">
        /// <param name="page">
        /// <param name="pageSize">
        /// Cancellation token used to cancel the request.
        /// </param>
        /// <returns>
        /// An <see cref="ActionResult"/> containing a collection of <see cref="BookResponse"/> objects.
        /// </returns>
        [HttpGet("get-paginated")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Book>))]
        public async Task<ActionResult<PagedResult<BookResponse>>> GetAll(
            CancellationToken ct,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10
        )
        {
            var books = await _bookService.GetAll(ct, page, pageSize);
            return Ok(books);
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<LendResponse>> LendBook(
            [Required]
            [StringLength(
                17,
                MinimumLength = 10,
                ErrorMessage = "ISBN must have 10 to 13 characters."
            )]
                string isbn,
            CancellationToken ct
        )
        {
            _logger.LogInformation("Processing lend request for ISBN: {Isbn}", isbn);
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<LendResponse>> ReturnBook(
            [Required]
            [StringLength(
                17,
                MinimumLength = 10,
                ErrorMessage = "ISBN must have 10 to 13 characters."
            )]
                string isbn,
            CancellationToken ct
        )
        {
            _logger.LogInformation("Processing return request for ISBN: {Isbn}", isbn);
            var response = await _bookService.IncreaseAvailableBookAmount(ct, isbn);
            return Ok(response);
        }
    }
}
