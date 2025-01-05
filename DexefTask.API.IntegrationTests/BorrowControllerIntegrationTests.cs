using System.Net.Http.Json;
using System.Net;
using Newtonsoft.Json;
using DexefTask.BusinessLogic.DTO.BorrowedBook;
using DexefTask.API.IntegrationTests;
using Moq;
using System.Net.Http.Headers;
using DexefTask.BusinessLogic.Interfaces.IServices;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DexefTask.DataAccess.Models;

namespace DexefTask.IntegrationTests
{
    public class BorrowControllerIntegrationTests : IDisposable
    {
        private CustomWebApplicationFactory _factory;
        private HttpClient _client;
        private readonly Mock<IBorrowedBookService> _borrowServicesMock;

        public BorrowControllerIntegrationTests()
        {
            _factory = new CustomWebApplicationFactory();
            _client = _factory.CreateClient();
            _borrowServicesMock = _factory.borrowedBookServicesMock; 
        }

        private string GenerateMockJwtToken(string role)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes("GqTwt3m1QWtsD2U+rej/AIZysJ8pe9ihfA1uhrGJdq4="); // Replace with your secret key
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, "Abdallah Waled"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, "abdallah@gmail.com"),
                new Claim("uid", Guid.NewGuid().ToString()), // Ensure uid is included
                new Claim("roles", role)
            };
            var symmetricSecurityKey = new SymmetricSecurityKey(key);
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: "https://localhost:5294/",
                audience: "https://localhost:5294/",
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: signingCredentials);
            var tokenString = tokenHandler.WriteToken(jwtSecurityToken);

            Console.WriteLine($"Generated Token: {tokenString}"); // Print the generated token

            return tokenString;
        }

        [Fact]
        public async Task BorrowBook_ReturnsOk_WhenBookIsAvailable()
        {
            // Arrange
            var token = GenerateMockJwtToken("User");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var borrowRequest = new BorrowedBookToAdd
            {
                BookId = Guid.NewGuid().ToString(),
                BorrowedDate = DateTime.Now,
                ReturnDate = DateTime.Now.AddDays(7)
            };

            // Mock the service method to return true for successful borrowing
            _borrowServicesMock
                .Setup(s => s.BorrowBook(It.IsAny<BorrowedBookToAdd>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask); // Simulate a successful operation

            // Act
            var response = await _client.PostAsync($"/api/borrow/{borrowRequest.BookId}", JsonContent.Create(borrowRequest));

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task BorrowBook_ReturnsBadRequest_WhenBookIsNotAvailable()
        {
            // Arrange
            var token = GenerateMockJwtToken("User");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var borrowRequest = new BorrowedBookToAdd
            {
                BookId = Guid.NewGuid().ToString(),
                UserId = Guid.NewGuid().ToString(),
                BorrowedDate = DateTime.Now,
                ReturnDate = DateTime.Now.AddDays(7)
            };

            // Mock the service method to return false for unsuccessful borrowing
            _borrowServicesMock
     .Setup(s => s.BorrowBook(It.IsAny<BorrowedBookToAdd>(), It.IsAny<string>()))
     .ThrowsAsync(new InvalidOperationException("Book is not available."));
            // Act
            var response = await _client.PostAsync($"/api/borrow/{borrowRequest.BookId}", JsonContent.Create(borrowRequest));

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task GetBorrowedBooksByUser_ReturnsList_WhenBooksAreBorrowed()
        {
            // Arrange
            var token = GenerateMockJwtToken("User");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var userId = "user-id"; // Mock user ID

            var borrowedBooks = new List<BorrowedBook>
    {
        new BorrowedBook
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Book = new Book { Title = "Book 1" },
            BorrowedDate = DateTime.Now.AddDays(-5),
            ReturnDate = DateTime.Now.AddDays(5)
        },
        new BorrowedBook
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Book = new Book { Title = "Book 2" },
            BorrowedDate = DateTime.Now.AddDays(-10),
            ReturnDate = DateTime.Now.AddDays(10)
        }
    };

            // Mock the service method to return a list of borrowed books
            _borrowServicesMock
                .Setup(s => s.GetBorrowedBooksByUser(It.IsAny<string>()))
                .ReturnsAsync(borrowedBooks.Select(b => new BorrowedBookToReturnDTO
                {
                    Id = b.Id,
                    UserName = "User Name",
                    BookName = b.Book.Title,
                    BorrowedDate = b.BorrowedDate,
                    ReturnDate = b.ReturnDate
                }).ToList());

            // Act
            var response = await _client.GetAsync($"/api/borrow");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var data = JsonConvert.DeserializeObject<List<BorrowedBookToReturnDTO>>(await response.Content.ReadAsStringAsync());
            Assert.Equal(2, data.Count);
            Assert.Equal("Book 1", data[0].BookName);
            Assert.Equal("Book 2", data[1].BookName);
        }
        public void Dispose()
        {
            _client.Dispose();
            _factory.Dispose();
        }
    }
}