using System.Linq.Expressions;
using global::BookManagement.Common.Exceptions;
using global::BookManagement.DbService;
using global::BookManagement.DbService.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BookManagement.DbService
{
    public class DbService<T> : IDbService<T>
        where T : class
    {
        protected readonly DataContext _context;
        private readonly ILogger<DbService<T>> _logger;

        public DbService(ILogger<DbService<T>> logger, DataContext context)
        {
            _logger = logger;
            _context = context;
        }

        /// <inheritdoc />
        public async Task<T?> GetAsync(Expression<Func<T, bool>> predicate, CancellationToken ct)
        {
            _logger.LogInformation("Retrieving a book");
            return await _context.Set<T>().AsNoTracking().FirstOrDefaultAsync(predicate, ct);
        }

        /// <inheritdoc />
        public async Task<List<T>> GetAllAsync(
            CancellationToken ct,
            Expression<Func<T, bool>>? where = null
        )
        {
            _logger.LogInformation("Retrieving books");
            var query = where != null ? _context.Set<T>().Where(where) : _context.Set<T>();
            return await query.ToListAsync(ct);
        }

        public async Task<List<T>> GetAllAsync(
            CancellationToken ct,
            Expression<Func<T, bool>>? predicate = null,
            Expression<Func<T, object>>? orderBy = null,
            int? skip = null,
            int? take = null
        )
        {
            _logger.LogInformation("Retrieving books with pagination");
            IQueryable<T> query = _context.Set<T>().AsNoTracking();
            if (predicate != null)
                query = query.Where(predicate);

            if (orderBy != null)
                query = query.OrderBy(orderBy);
            else
                query = query.OrderBy(x => x);

            if (skip.HasValue)
                query = query.Skip(skip.Value);
            if (take.HasValue)
                query = query.Take(take.Value);

            return await query.ToListAsync(ct);
        }

        /// <inheritdoc />
        public async Task<T> CreateAsync(T entity, CancellationToken ct)
        {
            try
            {
                _logger.LogInformation("Creating a Book");
                await _context.Set<T>().AddAsync(entity, ct);
                await _context.SaveChangesAsync(ct);

                return entity;
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError("Database constraint violation");
                throw new FailedToCreateException(typeof(T), "Database constraint violation", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError("Unexpected error during creation");
                throw new FailedToCreateException(
                    typeof(T),
                    "Unexpected error during creation",
                    ex
                );
            }
        }

        /// <inheritdoc />
        public async Task<T?> DeleteAsync(Expression<Func<T, bool>> predicate, CancellationToken ct)
        {
            _logger.LogInformation("Deleting book");
            var entity = await _context.Set<T>().FirstOrDefaultAsync(predicate, ct);

            if (entity == null)
            {
                _logger.LogError("Book not found");
                throw new NotFoundException(typeof(T));
            }

            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync(ct);

            _logger.LogInformation("Book was deleted");
            return entity;
        }

        /// <inheritdoc />
        public async Task<T?> UpdateAsync(
            Expression<Func<T, bool>> predicate,
            T updatedEntity,
            CancellationToken ct
        )
        {
            _logger.LogInformation("Updating book");
            var existingEntity = await _context.Set<T>().FirstOrDefaultAsync(predicate, ct);

            if (existingEntity == null)
            {
                _logger.LogError("Book not found");
                throw new NotFoundException(typeof(T));
            }

            _context.Entry(existingEntity).CurrentValues.SetValues(updatedEntity);
            await _context.SaveChangesAsync(ct);

            _logger.LogInformation("Book was updated");
            return existingEntity;
        }
    }
}
