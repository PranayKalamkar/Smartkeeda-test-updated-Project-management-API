using MySql.Data.MySqlClient;
using ProjectManagementAPI.Models;
using System.Data;

namespace ProjectManagementAPI.Data
{
    public class TaskRepository
    {
        private readonly IConfiguration _config;
        public TaskRepository(IConfiguration config) => _config = config;

        public async Task<List<GetProjectTaskModel>> GetByProjectAsync(int projectId)
        {
            var tasks = new List<GetProjectTaskModel>();
            List<GetUserModel> developers = new();
            try
            {
                using var conn = MySqlConnectionFactory.CreateConnection(_config);
                await conn.OpenAsync();

                using var cmd = new MySqlCommand("sp_getTasksByProjectId", conn) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.AddWithValue("@ProjectId", projectId);

                using var reader = await cmd.ExecuteReaderAsync();

                string? projectName = null;

                // 1. Project info
                if (await reader.ReadAsync())
                    projectName = reader.GetString("name");

                // 2. Move to task list
                if (await reader.NextResultAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        tasks.Add(new GetProjectTaskModel
                        {
                            Id = reader.GetInt32("id"),
                            Title = reader.GetString("title"),
                            Description = reader.GetString("description"),
                            ProjectId = reader.GetInt32("projectId"),
                            ProjectName = projectName,
                            AssignedTo = reader.GetInt32("assignToId"),
                            AssignedToName = reader.GetString("assignedToName"),
                            Status = Enum.Parse<TaskModelStatus>(reader.GetString("status"), true),
                            CreatedAt = reader.GetDateTime("createdAt"),
                            DueDate = reader.IsDBNull("dueDate") ? null : reader.GetDateTime("dueDate"),
                            ProjectDevelopers = new List<GetUserModel>()
                        });
                    }
                }

                // 3. Move to developer list
                if (await reader.NextResultAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        developers.Add(new GetUserModel
                        {
                            Id = reader.GetInt32("id"),
                            Username = reader.GetString("username"),
                            Email = reader.GetString("email"),
                            CreatedAt = reader.GetDateTime("createdAt")
                        });
                    }
                }

                // Attach developer list to all tasks
                foreach (var task in tasks)
                {
                    task.ProjectDevelopers = developers;
                }

                return tasks;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task CreateAsync(ProjectTaskModel task)
        {
            try
            {
                using var conn = MySqlConnectionFactory.CreateConnection(_config);
                await conn.OpenAsync();

                using var cmd = new MySqlCommand("sp_createTask", conn) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.AddWithValue("@Title", task.Title);
                cmd.Parameters.AddWithValue("@Description", task.Description);
                cmd.Parameters.AddWithValue("@ProjectId", task.ProjectId);
                cmd.Parameters.AddWithValue("@AssignToId", task.AssignedTo);
                cmd.Parameters.AddWithValue("@Status", task.Status.ToString());
                cmd.Parameters.AddWithValue("@CreatedAt", task.CreatedAt);
                cmd.Parameters.AddWithValue("@DueDate", task.DueDate);

                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task UpdateAsync(int id, ProjectTaskModel task)
        {
            try
            {
                using var conn = MySqlConnectionFactory.CreateConnection(_config);
                await conn.OpenAsync();

                using var cmd = new MySqlCommand("sp_updateTask", conn) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@Title", task.Title);
                cmd.Parameters.AddWithValue("@Description", task.Description);
                cmd.Parameters.AddWithValue("@Status", task.Status.ToString());
                cmd.Parameters.AddWithValue("@DueDate", task.DueDate);

                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                using var conn = MySqlConnectionFactory.CreateConnection(_config);
                await conn.OpenAsync();

                using var cmd = new MySqlCommand("sp_deleteTask", conn) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.AddWithValue("@Id", id);
                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
