using DexefTask.BusinessLogic.Interfaces.IServices.Authentication;
using DexefTask.BusinessLogic.Services.Authentication;
using DexefTask.DataAccess.Models.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DexefTask.API.Controllers
{
    /// <summary>
    /// Controller for handling authentication-related operations such as user registration and login.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        /// <summary>
        /// Registers a new user in the system.
        /// </summary>
        /// <param name="model">The model containing the user's registration information.</param>
        /// <returns>A response indicating the success or failure of the registration process.</returns>
        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await authService.RegisterAsync(model);

            if (!result.IsAuthenticated)
                return BadRequest(result.Message);

            return Ok(new {userEmail=result.Email, userName = result.Username, 
                token = result.Token,expiresOn=result.ExpiresOn});
        }
        /// <summary>
        /// Logs in an existing user to the system and asks for the token.
        /// </summary>
        /// <param name="model">The model containing the user's login information.</param>
        /// <returns>A response containing the user's roles, email, username, token, and expiration details if login is successful.</returns>
        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await authService.LoginAsync(model);

            if (!result.IsAuthenticated)
                return BadRequest(result.Message);

            return Ok(new {roles =result.Roles.ToList(),
                userEmail = result.Email,
                userName = result.Username,
                token = result.Token,
                expiresOn = result.ExpiresOn
            });
        }
    }
}
