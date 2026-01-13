using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using BookManagement.Common.Entities;
using BookManagement.Common.Requests;
using BookManagement.Common.Responses;

namespace BookManagement.Services.Interfaces
{
    public interface IBookService
    {
        /// <summary>
        /// Retrieves a specific Book entity by isbn.
        /// </summary>
        /// <param name="isbn">isbn code</param>
        /// <returns>A specific <see cref="Book"/></returns>
        /// <exception cref="NotFoundException"></exception>
        Task<BookResponse?> GetByISBN(CancellationToken ct, string isbn);

        /// <summary>
        /// Retrieves a specific Book entity by name.
        /// </summary>
        /// <param name="name">name of the book</param>
        /// <returns>A specific <see cref="Book"/></returns>
        /// <exception cref="NotFoundException"></exception>
        Task<List<BookResponse>> GetByName(CancellationToken ct, string name);

        /// <summary>
        /// Retrieves a list of Book entities from a specific author.
        /// </summary>
        /// <param name="author">name of the author</param>
        /// <returns>A <see cref="List{T}"/> where T is of type <see cref="Book"/></returns>
        /// <exception cref="NotFoundException"></exception>
        Task<List<BookResponse>> GetAllByAuthor(CancellationToken ct, string author);

        /// <summary>
        /// Retrieves a list all Book entities.
        /// </summary>
        /// <returns>A <see cref="List{T}"/> where T is of type <see cref="Book"/></returns>
        /// <exception cref="NotFoundException"></exception>
        Task<List<BookResponse>> GetAll(CancellationToken ct);

        /// <summary>
        /// Creates a specific Book entity.
        /// </summary>
        /// <param name="request"><see cref="CreateBookRequest"/></param>
        /// <returns>A new created <see cref="Book"/></returns>
        /// <exception cref="FailedToCreateException"></exception>
        Task<BookResponse> CreateBook(CancellationToken ct, CreateBookRequest request);

        /// <summary>
        /// Decreases the amount of availables books for a <see cref="Book"/> with certain ISBN code.
        /// </summary>
        /// <param name="isbn">isbn code</param>
        /// <returns>An instance of <see cref="LendResponse"/></returns>
        /// <exception cref="FailedToLendExpection"></exception>
        Task<LendResponse> DecreaseAvailableBookAmount(CancellationToken ct, string isbn);

        /// <summary>
        /// Increases the amount of availables books for a <see cref="Book"/> with certain ISBN code.
        /// </summary>
        /// <param name="isbn">isbn code</param>
        /// <returns>An instance of <see cref="LendResponse"/></returns>
        /// <exception cref="FailedToLendExpection"></exception>
        Task<LendResponse> IncreaseAvailableBookAmount(CancellationToken ct, string isbn);
    }
}
