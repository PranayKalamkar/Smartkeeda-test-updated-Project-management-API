using MySql.Data.MySqlClient;
using Mysqlx.Crud;
using ProjectManagementAPI.Models;
using System.Data;

namespace ProjectManagementAPI.Data
{
    public class ProjectsRepository
    {
        private readonly IConfiguration _config;
        public ProjectsRepository(IConfiguration config)
        {
            _config = config;
        }

        public async Task<IEnumerable<GetProjectModel>> GetAllAsync()
        {
            var list = new List<GetProjectModel>();
            try
            {
                using var conn = MySqlConnectionFactory.CreateConnection(_config);
                await conn.OpenAsync();

                using var cmd = new MySqlCommand("sp_getallprojects", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                using var reader = await cmd.ExecuteReaderAsync();

                // 1. Load all projects with manager info
                while (await reader.ReadAsync())
                {
                    list.Add(new GetProjectModel
                    {
                        Id = reader.GetInt32("id"),
                        Name = reader.GetString("name"),
                        Description = reader.GetString("description"),
                        ProjectManagerId = reader.GetInt32("projectManagerId"),
                        ProjectManagerName = reader.GetString("projectManagerName"),
                        Status = Enum.Parse<ProjectStatus>(reader.GetString("status"), true),
                        CreatedAt = reader.GetDateTime("createdAt"),
                        Developers = new List<GetUserModel>()
                    });
                }

                // 2. Move to developer result set
                if (await reader.NextResultAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        int projectId = reader.GetInt32("projectId");

                        var dev = new GetUserModel
                        {
                            Id = reader.GetInt32("id"),
                            Username = reader.GetString("username"),
                            Email = reader.GetString("email"),
                            CreatedAt = reader.GetDateTime("createdAt")
                        };

                        var project = list.FirstOrDefault(p => p.Id == projectId);
                        project?.Developers?.Add(dev);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
            return list;
        }

        public async Task<GetProjectModel?> GetByIdAsync(int id)
        {
            try
            {
                using var conn = MySqlConnectionFactory.CreateConnection(_config);
                await conn.OpenAsync();

                using var cmd = new MySqlCommand("sp_getprojectById", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@Id", id);

                using var reader = await cmd.ExecuteReaderAsync();

                GetProjectModel? project = null;

                // 1st result set: project info with manager name
                if (await reader.ReadAsync())
                {
                    project = new GetProjectModel
                    {
                        Id = reader.GetInt32("id"),
                        Name = reader.GetString("name"),
                        Description = reader.GetString("description"),
                        ProjectManagerId = reader.GetInt32("projectManagerId"),
                        ProjectManagerName = reader.GetString("projectManagerName"),
                        Status = Enum.Parse<ProjectStatus>(reader.GetString("status"), true),
                        CreatedAt = reader.GetDateTime("createdAt"),
                        Developers = new List<GetUserModel>()
                    };
                }

                // Move to 2nd result set: developer list
                if (await reader.NextResultAsync() && project != null)
                {
                    while (await reader.ReadAsync())
                    {
                        project.Developers!.Add(new GetUserModel
                        {
                            Id = reader.GetInt32("id"),
                            Username = reader.GetString("username"),
                            Email = reader.GetString("email"),
                            CreatedAt = reader.GetDateTime("createdAt")
                        });
                    }
                }

                return project;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task CreateAsync(ProjectModel project)
        {
            try
            {
                using var conn = MySqlConnectionFactory.CreateConnection(_config);
                await conn.OpenAsync();

                int projectId = 0;

                using var cmd = new MySqlCommand("sp_createProject", conn) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.AddWithValue("@Name", project.Name);
                cmd.Parameters.AddWithValue("@Description", project.Description);
                cmd.Parameters.AddWithValue("@ProjectManagerId", project.ProjectManagerId);
                cmd.Parameters.AddWithValue("@Status", project.Status.ToString());
                cmd.Parameters.AddWithValue("@CreatedAt", project.CreatedAt);

                using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                    projectId = Convert.ToInt32(reader["ProjectId"]);
                else
                    throw new Exception("Failed to retrieve ProjectId");

                await reader.CloseAsync();

                // Insert developers into tbl_project_developers via SP
                foreach (var devId in project.DeveloperIds)
                {
                    using var devCmd = new MySqlCommand("sp_addProjectDeveloper", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    devCmd.Parameters.AddWithValue("@ProjectId", projectId);
                    devCmd.Parameters.AddWithValue("@DeveloperId", devId);
                    await devCmd.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task UpdateAsync(int id, ProjectModel project)
        {
            try
            {
                using var conn = MySqlConnectionFactory.CreateConnection(_config);
                await conn.OpenAsync();

                // Update project details
                using (var cmd = new MySqlCommand("sp_updateProject", conn) { CommandType = CommandType.StoredProcedure })
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.AddWithValue("@Name", project.Name);
                    cmd.Parameters.AddWithValue("@Description", project.Description);
                    cmd.Parameters.AddWithValue("@Status", project.Status.ToString());
                    await cmd.ExecuteNonQueryAsync();
                }

                // Delete existing developers
                using (var delCmd = new MySqlCommand("sp_deleteProjectDevelopers", conn) { CommandType = CommandType.StoredProcedure })
                {
                    delCmd.Parameters.AddWithValue("@ProjectId", id);
                    await delCmd.ExecuteNonQueryAsync();
                }

                // Insert updated developer list
                foreach (var devId in project.DeveloperIds)
                {
                    using var devCmd = new MySqlCommand("sp_addProjectDeveloper", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    devCmd.Parameters.AddWithValue("@ProjectId", id);
                    devCmd.Parameters.AddWithValue("@DeveloperId", devId);
                    await devCmd.ExecuteNonQueryAsync();
                }
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

                using var cmd = new MySqlCommand("sp_deleteProject", conn) { CommandType = CommandType.StoredProcedure };
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
