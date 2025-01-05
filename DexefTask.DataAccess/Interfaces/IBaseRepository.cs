using System.Linq.Expressions;

namespace DexefTask.DataAccess.Interfaces
{
    public interface IBaseRepository<T> where T : class
        {
            Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? criteria = null, int? skip=null
                    , int? take = null, string[]? includes = null,
                    Expression<Func<T, object>>? orderBy = null, string? orderByDirection ="Ascending");
            Task<T> GetByIdAsync(Guid id);
            Task AddAsync(T entity);
            Task Update(T entity);
            Task Delete(Guid id);
        }

    
}
