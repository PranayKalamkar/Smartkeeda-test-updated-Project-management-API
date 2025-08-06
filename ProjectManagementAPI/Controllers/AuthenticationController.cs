using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectManagementAPI.Data;
using ProjectManagementAPI.Helpers;
using ProjectManagementAPI.Models;

namespace ProjectManagementAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly AuthenticationRepository _authenticationRepository;
        public AuthenticationController(IConfiguration configuration)
        {
            _config = configuration;
            _authenticationRepository = new AuthenticationRepository(_config);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var existingUser = await _authenticationRepository.GetUserByEmail(model.Email);
                if (existingUser != null)
                    return Conflict("User already exists.");

                var registrationResult = await _authenticationRepository.Register(model);

                if (registrationResult == 0)
                    return StatusCode(500, "Registration failed due to a server error.");

                return Ok("User registered successfully!");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var user = await _authenticationRepository.Login(model);
                if (user == null)
                    return Unauthorized("Invalid Credentials!");

                var token = JwtTokenHelper.GenerateToken(user, _config);
                return Ok(new { token });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}
