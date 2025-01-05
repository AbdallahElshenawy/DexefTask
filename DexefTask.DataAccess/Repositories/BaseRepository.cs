using System.Linq.Expressions;

namespace DexefTask.DataAccess.Repositories
{
    public class BaseRepository<T>(ApplicationDbContext context) : IBaseRepository<T> where T : class
    {
        public async Task AddAsync(T entity)
        {
            await context.Set<T>().AddAsync(entity);
        }

        public async Task Delete(Guid id)
        {
            var entity = await context.Set<T>().FindAsync(id);
            context.Set<T>().Remove(entity);

        }

        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? criteria, int? skip, int? take, string[]? includes = null, Expression<Func<T, object>>? orderBy = null, string? orderByDirection = "Ascending")
        {
            IQueryable<T> query = context.Set<T>();
            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            // Apply Criteria
            if (criteria != null)
            {
                query = query.Where(criteria);
            }
            
            // Apply Order By
            if (orderBy != null)
            {
                query = orderByDirection == "Ascending"
                    ? query.OrderBy(orderBy)
                    : query.OrderByDescending(orderBy);
            }

            // Apply Pagination
            if (skip.HasValue)
            {
                query = query.Skip(skip.Value);
            }

            if (take.HasValue)
            {
                query = query.Take(take.Value);
            }

            return await query.ToListAsync();
        }



        public async Task<T> GetByIdAsync(Guid id)
        {
            return await context.Set<T>().FindAsync(id);
        }

        public async Task Update(T entity)
        {
             context.Set<T>().Update(entity);
        }
    }
}
