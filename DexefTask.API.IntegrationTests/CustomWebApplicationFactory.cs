using DexefTask.BusinessLogic.Interfaces.IServices;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq; 

namespace DexefTask.API.IntegrationTests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        public Mock<IBookServices> BookServicesMock { get; }
        public Mock<IBorrowedBookService> borrowedBookServicesMock { get; }

        public CustomWebApplicationFactory()
        {
            BookServicesMock = new Mock<IBookServices>();
            borrowedBookServicesMock = new Mock<IBorrowedBookService>();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);

            builder.ConfigureTestServices(services =>
            {
                services.AddSingleton(BookServicesMock.Object);
                services.AddSingleton(borrowedBookServicesMock.Object);

            });
        }
    }
}