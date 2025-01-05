namespace DexefTask.DataAccess.Models
{
    public class Book
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public string? Author { get; set; }
        public string? Genre { get; set; }
        public int PublishedYear { get; set; }
        private bool _isAvailable; 

        public bool IsAvailable
        {
            get
            {
                // Find the last borrowed book based on the latest ReturnDate
                var lastBorrowedBook = BorrowedBooks
                    .OrderByDescending(bb => bb.ReturnDate)
                    .FirstOrDefault();

                // If no borrowed books exist, or the last one has been returned
                return lastBorrowedBook == null || lastBorrowedBook.ReturnDate <= DateTime.Now;
            }
            set { _isAvailable = value; }
            
        }
        public virtual ICollection<BorrowedBook> BorrowedBooks { get; set; } = new List<BorrowedBook>();

    }
}
