using AutoMapper;
using DexefTask.BusinessLogic.DTO.Book;
using DexefTask.BusinessLogic.Services;
using DexefTask.DataAccess.Interfaces;
using DexefTask.DataAccess.Models;
using Moq;
using System.Linq.Expressions;

namespace DexefTask.BusinessLogic.Tests
{
    public class BookServicesTests
    {

        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMapper> _mockMapper;
        private readonly BookServices _bookServices;

        public BookServicesTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _bookServices = new BookServices(_mockUnitOfWork.Object, _mockMapper.Object);
        }
        //Test Add Book
        [Fact]
        public async Task AddBook_ShouldAddBook_WhenBookIsValid()
        {
            // Arrange
            var bookToAdd = new BookToAddDTO { Title = "New Book",Author="Author" };
            var bookEntity = new Book { Title = "New Book", Author = "Author" };
            var bookToReturn = new BookToReturnDTO { Title = "New Book", Author = "Author" };

            _mockMapper.Setup(m => m.Map<Book>(bookToAdd)).Returns(bookEntity);
            _mockUnitOfWork.Setup(u => u.Books.GetAllAsync(null,null,null,null,null,null))
                .ReturnsAsync(new List<Book>());
            _mockUnitOfWork.Setup(u => u.Books.AddAsync(bookEntity)).Returns(Task.CompletedTask);
            _mockMapper.Setup(m => m.Map<BookToReturnDTO>(bookEntity)).Returns(bookToReturn);

            // Act
            var result = await _bookServices.AddBook(bookToAdd);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("New Book", result.Title);
            Assert.Equal("Author", result.Author);
        }
        [Fact]
        public async Task AddBook_ShouldThrowException_WhenBookIsNull()
        {
            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _bookServices.AddBook(null));
        }
        [Fact]
        public async Task AddBook_ShouldThrowException_WhenBookAlreadyExists()
        {
            // Arrange
            var bookToAdd = new BookToAddDTO { Title = "Existing Book" };
            var existingBook = new Book { Title = "Existing Book" };

            // Mock GetAllAsync to simulate a match for the predicate
            _mockUnitOfWork.Setup(u => u.Books.GetAllAsync(It.Is<Expression<Func<Book, bool>>>(predicate =>
                predicate.Compile()(existingBook)), null, null, null, null, "Ascending"))
                .ReturnsAsync(new List<Book> { existingBook });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _bookServices.AddBook(bookToAdd));
            Assert.Equal("Book already exists in database", exception.Message);

            // Verify GetAllAsync was called once
            _mockUnitOfWork.Verify(u => u.Books.GetAllAsync(It.IsAny<Expression<Func<Book, bool>>>(), null, null, null, null, "Ascending"), Times.Once);
        }


        // Test Get All
        [Fact]
        public async Task GetAllBooks_ShouldReturnAllBooks()
        {
            // Arrange
            var books = new List<Book>
            {
                new Book { Title = "Book 1" },
                new Book { Title = "Book 2" }
            };
                    var bookDtos = new List<BookToReturnDTO>
            {
                new BookToReturnDTO { Title = "Book 1" },
                new BookToReturnDTO { Title = "Book 2" }
            };

            // Mock GetAllAsync to return the list of books with optional parameters
            _mockUnitOfWork.Setup(u => u.Books.GetAllAsync(null, null, null, null, null, "Ascending"))
                .ReturnsAsync(books);

            // Mock the mapper to return the expected DTOs
            _mockMapper.Setup(m => m.Map<List<BookToReturnDTO>>(books))
                .Returns(bookDtos);

            // Act
            var result = await _bookServices.GetAllBooks();

            // Verify that GetAllAsync was called with the expected parameters
            _mockUnitOfWork.Verify(u => u.Books.GetAllAsync(null, null, null, null, null, "Ascending"), Times.Once);
            _mockMapper.Verify(m => m.Map<List<BookToReturnDTO>>(books), Times.Once);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("Book 1", result[0].Title);
            Assert.Equal("Book 2", result[1].Title);
        }
        [Fact]
        public async Task GetAllBooks_ShouldReturnEmptyList_WhenNoBooksExist()
        {
            // Arrange
            var emptyBooks = new List<Book>();

            // Mock GetAllAsync to return an empty list
            _mockUnitOfWork.Setup(u => u.Books.GetAllAsync(null, null, null, null, null, "Ascending"))
                .ReturnsAsync(emptyBooks);

            // Mock the mapper to return an empty list when mapping an empty list
            _mockMapper.Setup(m => m.Map<List<BookToReturnDTO>>(emptyBooks))
                .Returns(new List<BookToReturnDTO>());

            // Act
            var result = await _bookServices.GetAllBooks();

            // Assert
            Assert.NotNull(result); 
            Assert.Empty(result);   
        }

        //Get By Id
        [Fact]
        public async Task GetBookById_ShouldReturnBook_WhenBookExists()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            var existingBook = new Book { Id = bookId, Title = "Existing Book" };
            var bookDto = new BookToReturnDTO { Title = "Existing Book" };

            // Mock GetByIdAsync to return the existing book
            _mockUnitOfWork.Setup(u => u.Books.GetByIdAsync(bookId))
                .ReturnsAsync(existingBook);

            // Mock the mapper to return the expected DTO
            _mockMapper.Setup(m => m.Map<BookToReturnDTO>(existingBook))
                .Returns(bookDto);

            // Act
            var result = await _bookServices.GetBookById(bookId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Existing Book", result.Title);
        }
        [Fact]
        public async Task GetBookById_ShouldThrowException_WhenBookDoesNotExist()
        {
            // Arrange
            var bookId = Guid.NewGuid();

            // Mock GetByIdAsync to return null (book not found)
            _mockUnitOfWork.Setup(u => u.Books.GetByIdAsync(bookId))
                .ReturnsAsync((Book)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _bookServices.GetBookById(bookId));
            Assert.Equal("Book not found.", exception.Message);
        }
        // Edit Book
        [Fact]
        public async Task EditBook_ShouldEditBook_WhenBookExists()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            var existingBook = new Book { Id = bookId, Title = "Old Title" };
            var bookToEdit = new BookToAddDTO { Title = "New Title" };

            // Mock GetByIdAsync to return the existing book
            _mockUnitOfWork.Setup(u => u.Books.GetByIdAsync(bookId))
                .ReturnsAsync(existingBook);

            // Mock the mapper to map the new details to the existing book
            _mockMapper.Setup(m => m.Map(bookToEdit, existingBook));

            // Act
            await _bookServices.EditBook(bookId, bookToEdit);

            // Assert
            _mockUnitOfWork.Verify(u => u.Books.Update(existingBook), Times.Once);
            _mockUnitOfWork.Verify(u => u.Complete(), Times.Once);
        }
        [Fact]
        public async Task EditBook_ShouldThrowException_WhenBookDoesNotExist()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            var bookToEdit = new BookToAddDTO { Title = "New Title" };

            // Mock GetByIdAsync to return null (book not found)
            _mockUnitOfWork.Setup(u => u.Books.GetByIdAsync(bookId))
                .ReturnsAsync((Book)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _bookServices.EditBook(bookId, bookToEdit));
            Assert.Equal("Book not found.", exception.Message);
        }
        //Delete Book
        [Fact]
        public async Task DeleteBookById_ShouldDeleteBook_WhenBookExists()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            var existingBook = new Book { Id = bookId, Title = "Existing Book" };

            // Mock GetByIdAsync to return the existing book
            _mockUnitOfWork.Setup(u => u.Books.GetByIdAsync(bookId))
                .ReturnsAsync(existingBook);

            // Act
            await _bookServices.DeleteBookById(bookId);

            // Assert
            _mockUnitOfWork.Verify(u => u.Books.Delete(bookId), Times.Once);
            _mockUnitOfWork.Verify(u => u.Complete(), Times.Once);
        }
        [Fact]
        public async Task DeleteBookById_ShouldThrowException_WhenBookDoesNotExist()
        {
            // Arrange
            var bookId = Guid.NewGuid();

            // Mock GetByIdAsync to return null (book not found)
            _mockUnitOfWork.Setup(u => u.Books.GetByIdAsync(bookId))
                .ReturnsAsync((Book)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _bookServices.DeleteBookById(bookId));
            Assert.Equal("book not found.", exception.Message);
        }




    }
}
