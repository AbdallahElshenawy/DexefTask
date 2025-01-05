
namespace DexefTask.DataAccess.Models
{
    public class BorrowedBook
    {
        public Guid Id { get; set; }
        public Guid BookId { get; set; }
        public string? UserId { get; set; }
        public DateTime BorrowedDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public virtual Book? Book { get; set; } 
        public virtual User? User { get; set; }
    }
}
