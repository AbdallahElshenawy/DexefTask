using System.ComponentModel.DataAnnotations;

namespace DexefTask.BusinessLogic.DTO.BorrowedBook
{
    public class BorrowedBookToAdd
    {
        public string? BookId { get; set; }
        public string? UserId { get; set; }
        public DateTime BorrowedDate { get; set; } 
        public DateTime ReturnDate { get; set; } 
    }
}
