using DexefTask.BusinessLogic.DTO.Book;
using System.Linq.Expressions;

namespace DexefTask.BusinessLogic.Interfaces.IServices
{
    public interface IBookServices
    {
        Task<BookToReturnDTO> AddBook(BookToAddDTO book);
        Task<List<BookToReturnDTO>> GetAllBooks(Expression<Func<BookToReturnDTO, bool>> criteria=null, int? skip=null, int? take = null, string[]? includes = null,
            Expression<Func<BookToReturnDTO, object>>? orderBy = null, string orderByDirection = "Ascending");
        Task DeleteBookById(Guid id);
        Task EditBook(Guid id, BookToAddDTO book);

        Task<BookToReturnDTO> GetBookById(Guid id);
    }
}
