using DexefTask.BusinessLogic.DTO.BorrowedBook;
using DexefTask.BusinessLogic.Interfaces.IServices;
using DexefTask.BusinessLogic.Services;
using DexefTask.DataAccess.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DexefTask.API.Controllers
{
    /// <summary>
    /// Controller for handling book borrowing operations.
    /// Users can borrow books and view their borrowed books.
    /// </summary>
    [Authorize(Roles = "User")]

    [Route("api/[controller]")]
    [ApiController]
    public class BorrowController(IBorrowedBookService borrowedBookService) : ControllerBase
    {

        /// <summary>
        /// Allows a user to borrow a book.
        /// </summary>
        /// <param name="borrowedBook">The book dto borrowed.</param>
        /// <returns>A response indicating whether the book borrowing was successful or not.</returns>
        [HttpPost("{bookId}")]
        public async Task<IActionResult> BorrowBook([FromRoute] string bookId, [FromBody] BorrowedBookToAdd borrowedBook)
        {
            var userId = User.FindFirst("uid")!.Value;
            var borrowedBookDTO = new BorrowedBookToAdd
            {
                BookId = bookId,
                UserId = userId,
                BorrowedDate = borrowedBook.BorrowedDate,
                ReturnDate = borrowedBook.ReturnDate
            };

            try
            {
                await borrowedBookService.BorrowBook(borrowedBookDTO, userId);
                return Ok("Book borrowed successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// Retrieves all books that the loged in user has borrowed.
        /// </summary>
        /// <returns>A list of borrowed books.</returns>
        [HttpGet]
        public async Task<IActionResult> GetBorrowedBooks()
        {
            var userId = User.FindFirst("uid")!.Value;

            var borrowedBooks = await borrowedBookService.GetBorrowedBooksByUser(userId);
            return Ok(borrowedBooks);
        }
    }
}