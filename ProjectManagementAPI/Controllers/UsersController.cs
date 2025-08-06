using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectManagementAPI.Data;
using ProjectManagementAPI.Models;
using System.Security.Claims;

namespace ProjectManagementAPI.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly UsersRepository _repo;

        public UsersController(IConfiguration config)
        {
            _config = config;
            _repo = new UsersRepository(config);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var role = User.FindFirst(ClaimTypes.Role)?.Value;
                if (role != "admin")
                    return Forbid("Only admins allowed");

                var users = await _repo.GetAllAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [Authorize]
        [HttpPut("{id}/role")]
        public async Task<IActionResult> UpdateUserRole(int id, [FromBody] UpdateUserRoleRequest request)
        {
            try
            {
                var role = User.FindFirst(ClaimTypes.Role)?.Value;
                if (role != "admin")
                    return Forbid("Only admins allowed");

                await _repo.UpdateRoleAsync(id, request.Role);
                return Ok("User role updated");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [Authorize]
        [HttpGet("profile")]
        public IActionResult GetProfile()
        {
            try
            {
                var email = User.Identity?.Name;
                var role = User.FindFirst(ClaimTypes.Role)?.Value;
                var userId = User.FindFirst("UserId")?.Value;

                if (email == null || role == null || userId == null)
                    return Unauthorized("Invalid token.");

                return Ok(new
                {
                    Id = userId,
                    Email = email,
                    Role = role
                });
            }
            catch (Exception ex)
            {
                // Optionally log this exception using ILogger
                Console.WriteLine($"Error in GetProfile: {ex.Message}");
                return StatusCode(500, "An error occurred while fetching user profile.");
            }
        }

    }
}
