using MySql.Data.MySqlClient;
using ProjectManagementAPI.Models;
using System.Data;

namespace ProjectManagementAPI.Data
{
    public class UsersRepository
    {
        private readonly IConfiguration _config;
        public UsersRepository(IConfiguration config)
        {
            _config = config;
        }

        public async Task<List<UserModel>> GetAllAsync()
        {
            List<UserModel> users = new();
            try
            {
                using var conn = MySqlConnectionFactory.CreateConnection(_config);
                await conn.OpenAsync();

                using var cmd = new MySqlCommand("sp_getallusers", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    users.Add(new UserModel
                    {
                        Id = reader.GetInt32("id"),
                        Username = reader.GetString("username"),
                        Email = reader.GetString("email"),
                        PasswordHash = reader.GetString("passwordHash"),
                        Role = Enum.Parse<UserRole>(reader.GetString("role"), true),
                        CreatedAt = reader.GetDateTime("createdAt")
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
            return users;
        }

        public async Task UpdateRoleAsync(int id, string role)
        {
            try
            {
                using var conn = MySqlConnectionFactory.CreateConnection(_config);
                await conn.OpenAsync();

                using var cmd = new MySqlCommand("sp_updateuserrole", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@UserId", id);
                cmd.Parameters.AddWithValue("@Role", role);

                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
