namespace DexefTask.BusinessLogic.DTO.BorrowedBook
{
    public class BorrowedBookToReturnDTO
    {
        public Guid Id { get; set; }
        public string? BookName { get; set; }
        public string? UserName { get; set; }
        public DateTime BorrowedDate { get; set; }
        public DateTime ReturnDate { get; set; }
    }
}
