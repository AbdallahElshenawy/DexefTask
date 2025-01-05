using DexefTask.BusinessLogic.DTO.BorrowedBook;
using DexefTask.BusinessLogic.Interfaces.IServices;
using DexefTask.DataAccess.Interfaces;
using DexefTask.DataAccess.Models;

namespace DexefTask.BusinessLogic.Services
{
    public class BorrowedBookService(IUnitOfWork unitOfWork) : IBorrowedBookService
    {
        /// <summary>
        /// Allows a user to borrow a book.
        /// </summary>
        /// <param name="borrowedBookDTO">Details of the book to be borrowed.</param>
        /// <param name="userId">The ID of the user borrowing the book.</param>
        /// <exception cref="Exception">Thrown if the book is not found, unavailable, the return date is invalid, or if the user violates borrowing rules.</exception>
        public async Task BorrowBook(BorrowedBookToAdd borrowedBookDTO, string userId)
        {
            var book = await unitOfWork.Books.GetByIdAsync(Guid.Parse(borrowedBookDTO.BookId!));

            if (book == null)
                throw new Exception("Book not found.");

            // get the last book that the user borrowed
            var lastBorrowedBook = await unitOfWork.BorrowedBooks.GetAllAsync(
                criteria: b => b.UserId == userId && b.BookId == book.Id,
                skip: null,
                take: 1,
                includes: null,
                orderBy: b => b.ReturnDate,
                orderByDirection: "Descending"
            );
            var lastBorrowedBookValue = lastBorrowedBook.FirstOrDefault();

            // If the book is still not available, throw an exception
            if (!book.IsAvailable)
                throw new InvalidOperationException("Book is not available.");
            // Update the availability of the book based on existing borrowed records

            if ((lastBorrowedBookValue != null) && (lastBorrowedBookValue.BookId == book.Id) )
            {
                throw new InvalidOperationException("The same book cannot be borrowed twice.");
            }

            // Check if the return date is valid (not more than 14 days from the borrowed date)
            if ((borrowedBookDTO.ReturnDate - borrowedBookDTO.BorrowedDate).TotalDays > 14)
            {
                throw new Exception("The return date must be within 14 days from the borrowed date.");
            }

            var borrowedBook = new BorrowedBook
            {
                Id = Guid.NewGuid(),
                BookId = book.Id,
                UserId = userId,
                BorrowedDate = borrowedBookDTO.BorrowedDate,
                ReturnDate = borrowedBookDTO.ReturnDate
            };

            await unitOfWork.BorrowedBooks.AddAsync(borrowedBook);
            book.IsAvailable = false; 
            await unitOfWork.Complete();
        }

        /// <summary>
        /// Retrieves a list of borrowed books for a specific user.
        /// </summary>
        /// <param name="userId">The ID of the user whose borrowed books are to be retrieved.</param>
        /// <returns>A list of borrowed books with details such as username, book title, borrowed date, and return date.</returns>
        public async Task<List<BorrowedBookToReturnDTO>> GetBorrowedBooksByUser(string userId)
        {
            var borrowedBooks = await unitOfWork.BorrowedBooks.GetAllAsync(b => b.UserId == userId);

            if (!borrowedBooks.Any())
                return new List<BorrowedBookToReturnDTO>(); // Return an empty list if no books are found

            var borrowedBookDTO = borrowedBooks.Select(b => new BorrowedBookToReturnDTO
            {
                Id= b.Id,
                UserName = b.User.UserName,
                BookName = b.Book.Title,
                BorrowedDate = b.BorrowedDate,
                ReturnDate = b.ReturnDate
            }).ToList();

            return borrowedBookDTO;
        }
    }
}