using DexefTask.DataAccess.Repositories;
using Microsoft.Extensions.Hosting;

namespace DexefTask.DataAccess
{
    /// <summary>
    /// Provides extension method for registering data access services.
    /// </summary>
    public static class DataAccessRegistration
    {
        /// <summary>
        /// Configures data access services for the application.
        /// </summary>
        /// <param name="builder">The <see cref="WebApplicationBuilder"/> used to configure the application.</param>
        public static void AddDataAccessRegistration(this WebApplicationBuilder builder)
        {
            if (builder.Environment.IsEnvironment("Testing"))
            {
                /// <summary>
                /// Configures the application to use an in-memory database for testing environments.
                /// </summary>
                builder.Services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryDbForTesting");
                });
            }
            else
            {
                /// <summary>
                /// Configures the application to use SQL Server for non-testing environments.
                /// </summary>
                builder.Services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
                           .UseLazyLoadingProxies();
                });
            }

            /// <summary>
            /// Configures Identity services with default token providers and EF Core stores.
            /// </summary>
            builder.Services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            /// <summary>
            /// Registers generic repository and unit of work patterns.
            /// </summary>
            builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
        }
    }
}
