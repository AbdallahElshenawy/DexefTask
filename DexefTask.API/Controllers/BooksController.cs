using DexefTask.BusinessLogic.DTO.Book;
using DexefTask.BusinessLogic.Interfaces.IServices;
using DexefTask.BusinessLogic.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace DexefTask.API.Controllers
{
    /// <summary>
    /// Controller for managing books in the system.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController(IBookServices bookServices, IMemoryCache cache) : ControllerBase
    {
       
        /// <summary>
        /// Retrieves all books from the system, using caching to improve performance.
        /// </summary>
               
        [Authorize(Roles = "Admin,User")]
        [HttpGet]

        public async Task<IActionResult> GetAll()
        {
            const string cacheKey = "allBooks";
            try
            {
                // Check if the books are already cached
                if (!cache.TryGetValue(cacheKey, out var books))
                {
                    // If not cached, retrieve from the service
                    books = await bookServices.GetAllBooks();

                    // Set cache options
                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromMinutes(5)); // Cache for 5 minutes

                    // Save data in cache
                    cache.Set(cacheKey, books, cacheEntryOptions);
                }

                return Ok(books);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving books.", error = ex.Message });
            }
        }
        /// <summary>
        /// Adds a new book to the system.
        /// </summary>
        /// <param name="book">The details of the book to be added.</param>
        /// <returns>The added book.</returns>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] BookToAddDTO book)
        {
            if (book == null)
                return BadRequest("Book information must be provided.");

            if (ModelState.IsValid)
            {
                try
                {
                    var response = await bookServices.AddBook(book);
                    return Created($"/api/books", response);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("Error", ex.Message);
                }
            }
            return BadRequest(ModelState);
        }
        /// <summary>
        /// Retrieves a book by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the book.</param>
        /// <returns>The book with the specified ID.</returns>

        [Authorize(Roles = "Admin,User")]

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var book = await bookServices.GetBookById(id);
                return Ok(book);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
        /// <summary>
        /// Deletes a book by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the book to be deleted.</param>
        /// <returns>Confirmation message on successful deletion.</returns>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await bookServices.DeleteBookById(id);
                return Ok(new { message = "Book deleted successfully" });
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
        /// <summary>
        /// Updates the information of an existing book.
        /// </summary>
        /// <param name="id">The unique identifier of the book to be updated.</param>
        /// <param name="book">The new details of the book.</param>
        /// <returns>Confirmation message on successful update.</returns>
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(Guid id, [FromBody] BookToAddDTO book)
        {
            if (book == null)
                return BadRequest("Book information must be provided.");

            try
            {
                await bookServices.EditBook(id, book);
                return Ok(new { message = "Book updated successfully" });
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
