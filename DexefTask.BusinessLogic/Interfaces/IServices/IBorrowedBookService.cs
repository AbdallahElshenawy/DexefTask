using DexefTask.BusinessLogic.DTO.BorrowedBook;

namespace DexefTask.BusinessLogic.Interfaces.IServices
{
    public interface IBorrowedBookService
    {
        Task BorrowBook(BorrowedBookToAdd borrowedBookDTO, string userId);
        Task<List<BorrowedBookToReturnDTO>> GetBorrowedBooksByUser(string userId);
      
    }
}
