using Xunit;
using Moq;
using DexefTask.BusinessLogic.Services;
using DexefTask.BusinessLogic.DTO.BorrowedBook;
using DexefTask.DataAccess.Interfaces;
using DexefTask.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DexefTask.BusinessLogic.Tests
{
    public class BorrowedBookServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly BorrowedBookService _borrowedBookService;

        public BorrowedBookServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _borrowedBookService = new BorrowedBookService(_unitOfWorkMock.Object);
        }

        [Fact]
        public async Task BorrowBook_Success()
        {
            // Arrange
            var borrowedBookDTO = new BorrowedBookToAdd
            {
                BookId = Guid.NewGuid().ToString(),
                BorrowedDate = DateTime.Now,
                ReturnDate = DateTime.Now.AddDays(7) 
            };
            var userId = Guid.NewGuid().ToString();

            var book = new Book
            {
                Id = Guid.NewGuid(),
                IsAvailable = true
            };

            _unitOfWorkMock.Setup(u => u.Books.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(book);
            _unitOfWorkMock.Setup(u => u.BorrowedBooks.GetAllAsync(It.IsAny<Expression<Func<BorrowedBook, bool>>>(), null, null, null, null, null)).ReturnsAsync(new List<BorrowedBook>());

            // Act
            await _borrowedBookService.BorrowBook(borrowedBookDTO, userId);

            // Assert
            _unitOfWorkMock.Verify(u => u.BorrowedBooks.AddAsync(It.IsAny<BorrowedBook>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.Complete(), Times.Once);
        }

        [Fact]
        public async Task BorrowBook_BookNotFound_ThrowsException()
        {
            // Arrange
            var borrowedBookDTO = new BorrowedBookToAdd { BookId = Guid.NewGuid().ToString() };
            var userId = Guid.NewGuid().ToString();

            _unitOfWorkMock.Setup(u => u.Books.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Book?)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _borrowedBookService.BorrowBook(borrowedBookDTO, userId));
        }

        [Fact]
        public async Task GetBorrowedBooksByUser_NoBooks_ReturnsEmptyList()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();

            _unitOfWorkMock.Setup(u => u.BorrowedBooks.GetAllAsync(It.IsAny<Expression<Func<BorrowedBook, bool>>>(), null, null, null, null, null)).ReturnsAsync(new List<BorrowedBook>());

            // Act
            var result = await _borrowedBookService.GetBorrowedBooksByUser(userId);

            // Assert
            Assert.Empty(result);
        }
    }
}