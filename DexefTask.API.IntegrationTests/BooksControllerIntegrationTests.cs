using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using DexefTask.BusinessLogic.DTO.Book;
using DexefTask.API.IntegrationTests;
using Newtonsoft.Json;
using System.Net;
using DexefTask.BusinessLogic.Interfaces.IServices;
using Moq;
using System.Net.Http.Headers;
using Microsoft.IdentityModel.Tokens;
using System.Linq.Expressions;

namespace DexefTask.IntegrationTests
{
    public class BooksControllerIntegrationTests : IDisposable
    {
        private CustomWebApplicationFactory _factory;
        private HttpClient _client;
        private readonly Mock<IBookServices> _bookServicesMock;

        public BooksControllerIntegrationTests()
        {
            _factory = new CustomWebApplicationFactory();
            _client = _factory.CreateClient();
            _bookServicesMock = _factory.BookServicesMock;
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
                new Claim("uid",Guid.NewGuid().ToString()),
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

                Console.WriteLine($"Error generating token: {tokenString}");

                return tokenString;
           
        }


        [Fact]
        public async Task GetAll_ReturnsOk_WhenBooksExist()
        {
            // Arrange
            var token = GenerateMockJwtToken("User");
            Console.WriteLine($"Generated Token for GetAll: {token}"); // Print the token
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var mockBooks = new List<BookToReturnDTO>
            {
                new BookToReturnDTO { Id = Guid.NewGuid().ToString(), Title = "Existing Book 1", Author = "Author 1", Genre = "fancy", PublishedYear = 2020, IsAvailable = true },
                new BookToReturnDTO { Id = Guid.NewGuid().ToString(), Title = "Existing Book 2", Author = "Author 2", Genre = "fancy", PublishedYear = 2020, IsAvailable = true }
            };

            // Ensure correct setup for GetAllBooks
            _bookServicesMock.Setup(s => s.GetAllBooks(It.IsAny<Expression<Func<BookToReturnDTO, bool>>>(), It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<string[]>(), It.IsAny<Expression<Func<BookToReturnDTO, object>>>(), It.IsAny<string>())).ReturnsAsync(mockBooks);

            // Act
            var response = await _client.GetAsync("/api/books");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode); // Expecting OK
            var data = JsonConvert.DeserializeObject<IEnumerable<BookToReturnDTO>>(await response.Content.ReadAsStringAsync());

            Assert.NotNull(data);
            Assert.Collection(data,
                book =>
                {
                    Assert.Equal("Existing Book 1", book.Title);
                    Assert.Equal("Author 1", book.Author);
                },
                book =>
                {
                    Assert.Equal("Existing Book 2", book.Title);
                    Assert.Equal("Author 2", book.Author);
                });
        }

        [Fact]
        public async Task Add_ReturnsCreated_WhenBookIsValid()
        {
            // Arrange
            var token = GenerateMockJwtToken("Admin");
            Console.WriteLine($"Generated Token for Add: {token}"); // Print the token
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var newBook = new BookToAddDTO
            {
                Title = "New Book",
                Author = "New Author",
                Genre = "Fiction",
                PublishedYear = 2023,
                IsAvailable = true
            };
            var addedBook = new BookToReturnDTO
            {
                Title = newBook.Title,
                Author = newBook.Author,
                Genre = newBook.Genre,
                PublishedYear = newBook.PublishedYear,
                IsAvailable = newBook.IsAvailable
            };

            // Ensure correct setup for AddBook
            _bookServicesMock.Setup(s => s.AddBook(newBook)).ReturnsAsync(addedBook);

            // Act
            var response = await _client.PostAsync("/api/books", JsonContent.Create(newBook));

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
      
        }

        [Fact]
        public async Task Add_ReturnsBadRequest_WhenBookIsInvalid()
        {
            // Arrange
            var token = GenerateMockJwtToken("Admin");
            Console.WriteLine($"Generated Token for Add Bad Request: {token}"); // Print the token
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var invalidBook = new BookToAddDTO { Title = "", Author = "New Author" };

            // Act
            var response = await _client.PostAsync("/api/books", JsonContent.Create(invalidBook));

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Delete_ReturnsOk_WhenBookExists()
        {
            // Arrange
            var token = GenerateMockJwtToken("Admin");
            Console.WriteLine($"Generated Token for Delete: {token}"); // Print the token
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var bookId = Guid.NewGuid();
            _bookServicesMock.Setup(s => s.DeleteBookById(bookId)).Returns(Task.CompletedTask);

            // Act
            var response = await _client.DeleteAsync($"/api/books/{bookId}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenBookDoesNotExist()
        {
            // Arrange
            var token = GenerateMockJwtToken("Admin");
            Console.WriteLine($"Generated Token for Delete Not Found: {token}"); // Print the token
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var bookId = Guid.NewGuid();
            _bookServicesMock.Setup(s => s.DeleteBookById(bookId)).ThrowsAsync(new Exception("book not found."));

            // Act
            var response = await _client.DeleteAsync($"/api/books/{bookId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        public void Dispose()
        {
            _client.Dispose();
            _factory.Dispose();
        }
    }
}