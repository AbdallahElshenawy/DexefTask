using DexefTask.BusinessLogic.Helpers;
using DexefTask.BusinessLogic.Interfaces.IServices;
using DexefTask.BusinessLogic.Interfaces.IServices.Authentication;
using DexefTask.BusinessLogic.Services;
using DexefTask.BusinessLogic.Services.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace DexefTask.BusinessLogic
{
    public static class BusinessLogicRegistration
    {
        /// <summary>
        /// Registers the necessary business logic services and configurations for the application.
        /// </summary>
        /// <param name="builder">The WebApplicationBuilder used to configure the application.</param>
        public static void AddBusinessLogicRegistration(this WebApplicationBuilder builder)
        {
            // Register AutoMapper with the assembly containing mapping profiles.
            builder.Services.AddAutoMapper(typeof(BusinessLogicRegistration).Assembly);

            // Register business logic services.
            builder.Services.AddScoped<IBookServices, BookServices>();
            builder.Services.AddScoped<IBorrowedBookService, BorrowedBookService>();
            builder.Services.AddScoped<IAuthService, AuthService>();

            // Configure JWT options using the "JWT" section of the configuration.
            builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("JWT"));

            // Add and configure JWT authentication.
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                  .AddJwtBearer(o =>
                  {
                      o.RequireHttpsMetadata = false;
                      o.SaveToken = false;
                      o.TokenValidationParameters = new TokenValidationParameters
                      {
                          ValidateIssuerSigningKey = true,
                          ValidateIssuer = true,
                          ValidateAudience = true,
                          ValidateLifetime = true,
                          ValidIssuer = builder.Configuration["JWT:Issuer"],
                          ValidAudience = builder.Configuration["JWT:Audience"],
                          IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]))
                      };
                  });

        }
    }
}
