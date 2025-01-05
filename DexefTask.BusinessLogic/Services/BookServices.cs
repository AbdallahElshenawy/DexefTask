using AutoMapper;
using DexefTask.BusinessLogic.DTO.Book;
using DexefTask.BusinessLogic.Interfaces.IServices;
using DexefTask.DataAccess.Interfaces;
using DexefTask.DataAccess.Models;
using System.Linq.Expressions;
namespace DexefTask.BusinessLogic.Services
{
    public class BookServices(IUnitOfWork unitOfWork , IMapper mapper) : IBookServices
    {
        /// <summary>
        /// Adds a new book to the database.
        /// </summary>
        /// <param name="book">The details of the book to add.</param>
        /// <returns>A DTO representing the added book.</returns>
        /// <exception cref="Exception">Thrown if the book is null or already exists in the database.</exception>
        public async Task<BookToReturnDTO> AddBook(BookToAddDTO book)
        {
            if (book == null)
                throw new Exception("Book Can't Be Null");
            var bookinDb = await unitOfWork.Books.GetAllAsync(c => c.Title.ToLower() == book.Title.ToLower());
            if (bookinDb.Any())
                throw new Exception("Book already exists in database");

            var newBook = mapper.Map<Book>(book);
            await unitOfWork.Books.AddAsync(newBook);
            try
            {
                await unitOfWork.Complete();
            }
            catch (Exception)
            {
                throw new Exception("An error occured while creating the book");
            }

            return mapper.Map<BookToReturnDTO>(newBook);


        }
        /// <summary>
        /// Deletes a book from the database by its identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the book to delete.</param>
        /// <exception cref="Exception">Thrown if the book is not found.</exception>
        public async Task DeleteBookById(Guid id)
        {
            var book = await unitOfWork.Books.GetByIdAsync(id);
            if (book is null)
                throw new Exception("book not found.");

            await unitOfWork.Books.Delete(id);
            await unitOfWork.Complete();
        }
        /// <summary>
        /// Edits the details of an existing book.
        /// </summary>
        /// <param name="id">The unique identifier of the book to edit.</param>
        /// <param name="book">The updated book details.</param>
        /// <exception cref="Exception">Thrown if the book is not found.</exception>
        public async Task EditBook(Guid id, BookToAddDTO book)
        {
            var existingBook = await unitOfWork.Books.GetByIdAsync(id);

            if (existingBook is null)
                throw new Exception("Book not found.");

            mapper.Map(book, existingBook);

            await unitOfWork.Books.Update(existingBook);

            await unitOfWork.Complete();
        }
        /// <summary>
        /// Retrieves all books matching the specified criteria.
        /// </summary>
        /// <param name="criteria">The filtering criteria for books.</param>
        /// <param name="skip">The number of records to skip.</param>
        /// <param name="take">The number of records to take.</param>
        /// <param name="includes">Related entities to include in the query.</param>
        /// <param name="orderBy">The sorting criteria.</param>
        /// <param name="orderByDirection">The sorting direction (Ascending or Descending).</param>
        /// <returns>A list of books matching the specified criteria.</returns>
        public async Task<List<BookToReturnDTO>> GetAllBooks(Expression<Func<BookToReturnDTO, bool>> criteria=null, 
            int? skip=null, int? take=null, string[]? includes = null,
            Expression<Func<BookToReturnDTO, object>>? orderBy = null,
            string orderByDirection = "Ascending")
        {
            var books = await unitOfWork.Books.GetAllAsync();
            return mapper.Map<List<BookToReturnDTO>>(books);
        }
        /// <summary>
        /// Retrieves a book by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the book.</param>
        /// <returns>A DTO representing the book.</returns>
        /// <exception cref="Exception">Thrown if the book is not found.</exception>
        public async Task<BookToReturnDTO> GetBookById(Guid id)
        {
            var book = await unitOfWork.Books.GetByIdAsync(id);
            if (book == null)
                throw new Exception("Book not found.");

            return mapper.Map<BookToReturnDTO>(book);
        }
    }
}
