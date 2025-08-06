using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectManagementAPI.Data;
using ProjectManagementAPI.Models;
using System.Security.Claims;

namespace ProjectManagementAPI.Controllers
{
    [Route("api/projects")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly ProjectsRepository _repo;

        public ProjectsController(IConfiguration config)
        {
            _config = config;
            _repo = new ProjectsRepository(config);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var projects = await _repo.GetAllAsync();
                return Ok(projects);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(ProjectModel project)
        {
            try
            {
                var role = User.FindFirst(ClaimTypes.Role)?.Value;
                if (role != "admin" && role != "pm")
                    return Forbid("Only admins or pm allowed");
                await _repo.CreateAsync(project);
                return Ok("Project created successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ProjectModel project)
        {
            try
            {
                var role = User.FindFirst(ClaimTypes.Role)?.Value;
                if (role != "admin")
                    return Forbid("Only admins allowed");
                await _repo.UpdateAsync(id, project);
                return Ok("Project updated successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var role = User.FindFirst(ClaimTypes.Role)?.Value;
                if (role != "admin")
                    return Forbid("Only admins allowed");
                await _repo.DeleteAsync(id);
                return Ok("Project deleted");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var project = await _repo.GetByIdAsync(id);
                if (project == null) return NotFound();
                return Ok(project);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}
