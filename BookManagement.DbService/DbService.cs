using System.Linq.Expressions;
using global::BookManagement.Common.Exceptions;
using global::BookManagement.DbService;
using global::BookManagement.DbService.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookManagement.DbService
{
    public class DbService<T> : IDbService<T>
        where T : class
    {
        protected readonly DataContext _context;

        public DbService(DataContext context)
        {
            _context = context;
        }

        /// <inheritdoc />
        public async Task<T?> GetAsync(CancellationToken ct, Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().AsNoTracking().FirstOrDefaultAsync(predicate, ct);
        }

        /// <inheritdoc />
        public async Task<List<T>> GetAllAsync(
            CancellationToken ct,
            Expression<Func<T, bool>>? where = null
        )
        {
            var query = where != null ? _context.Set<T>().Where(where) : _context.Set<T>();
            return await query.ToListAsync(ct);
        }

        /// <inheritdoc />
        public async Task<T> CreateAsync(CancellationToken ct, T entity)
        {
            try
            {
                await _context.Set<T>().AddAsync(entity, ct);
                await _context.SaveChangesAsync(ct);

                return entity;
            }
            catch (DbUpdateException dbEx)
            {
                throw new FailedToCreateException(typeof(T), "Database constraint violation", dbEx);
            }
            catch (Exception ex)
            {
                throw new FailedToCreateException(
                    typeof(T),
                    "Unexpected error during creation",
                    ex
                );
            }
        }

        /// <inheritdoc />
        public async Task<T?> DeleteAsync(CancellationToken ct, Expression<Func<T, bool>> predicate)
        {
            var entity = await _context.Set<T>().FirstOrDefaultAsync(predicate, ct);

            if (entity == null)
                throw new NotFoundException(typeof(T));

            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync(ct);

            return entity;
        }

        /// <inheritdoc />
        public async Task<T?> UpdateAsync(
            CancellationToken ct,
            Expression<Func<T, bool>> predicate,
            T updatedEntity
        )
        {
            var existingEntity = await _context.Set<T>().FirstOrDefaultAsync(predicate, ct);

            if (existingEntity == null)
                throw new NotFoundException(typeof(T));

            _context.Entry(existingEntity).CurrentValues.SetValues(updatedEntity);
            await _context.SaveChangesAsync(ct);

            return existingEntity;
        }
    }
}
