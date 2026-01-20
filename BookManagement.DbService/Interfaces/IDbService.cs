using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BookManagement.DbService.Interfaces
{
    public interface IDbService<T>
    {
        /// <summary>
        /// retrieves a specific T entity.
        /// </summary>
        /// <param name="where">query</param>
        /// <returns>Entity</returns>
        Task<T?> GetAsync(Expression<Func<T, bool>> predicate, CancellationToken ct);

        /// <summary>
        /// retrieves a list of specific T entity.
        /// </summary>
        /// <param name="predicateToGetId">A query</param>
        /// <returns>Entity</returns>
        Task<List<T>> GetAllAsync(CancellationToken ct, Expression<Func<T, bool>>? where = null);

        /// <summary>
        /// retrieves a list of specific T entity.
        /// </summary>
        /// <param name="predicateToGetId">A query</param>
        /// <returns>Entity</returns>
        Task<List<T>> GetAllAsync(
            CancellationToken ct,
            Expression<Func<T, bool>>? predicate,
            Expression<Func<T, object>>? orderBy = null,
            int? skip = null,
            int? take = null
        );

        /// <summary>
        /// Creates a new T entity.
        /// </summary>
        /// <param name="entity">Entity to be created</param>
        /// <returns>true if created, else false</returns>
        Task<T> CreateAsync(T entity, CancellationToken ct);

        /// <summary>
        /// Updates a T entity.
        /// </summary>
        /// <param name="predicate">A query</param>
        /// <param name="updatedEntity">Entity with new values</param>
        /// <returns>true if updated, else false</returns>
        Task<T?> UpdateAsync(
            Expression<Func<T, bool>> predicate,
            T updatedEntity,
            CancellationToken ct
        );

        /// <summary>
        /// Deletes a T entity.
        /// </summary>
        /// <param name="predicate">A query</param>
        /// <returns>true if deleted, else false</returns>
        Task<T?> DeleteAsync(Expression<Func<T, bool>> predicate, CancellationToken ct);
    }
}
