using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectManagementAPI.Data;
using ProjectManagementAPI.Models;
using System.Security.Claims;

namespace ProjectManagementAPI.Controllers
{
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly TaskRepository _repo;

        public TasksController(IConfiguration config)
        {
            _config = config;
            _repo = new TaskRepository(config);
        }

        [HttpGet("api/projects/{projectId}/tasks")]
        public async Task<IActionResult> GetByProject(int projectId)
        {
            try
            {
                var tasks = await _repo.GetByProjectAsync(projectId);
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [Authorize]
        [HttpPost("api/projects/{projectId}/tasks")]
        public async Task<IActionResult> Create(int projectId, ProjectTaskModel task)
        {
            try
            {
                var role = User.FindFirst(ClaimTypes.Role)?.Value;
                if (role != "admin" && role != "pm")
                    return Forbid("Only admins or pm allowed");

                task.ProjectId = projectId;
                await _repo.CreateAsync(task);
                return Ok("Task created");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [Authorize]
        [HttpPut("api/tasks/{id}")]
        public async Task<IActionResult> Update(int id, ProjectTaskModel task)
        {
            try
            {
                var role = User.FindFirst(ClaimTypes.Role)?.Value;
                if (role != "admin" && role != "pm")
                    return Forbid("Only admins or pm allowed");

                await _repo.UpdateAsync(id, task);
                return Ok("Task updated");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [Authorize]
        [HttpDelete("api/tasks/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {  
                var role = User.FindFirst(ClaimTypes.Role)?.Value;
                if (role != "admin" && role != "pm")
                    return Forbid("Only admins or pm allowed");

                await _repo.DeleteAsync(id);
                return Ok("Task deleted");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

    }
}
