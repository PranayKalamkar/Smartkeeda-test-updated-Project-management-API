using MySql.Data.MySqlClient;
using ProjectManagementAPI.Helpers;
using ProjectManagementAPI.Models;
using System.Data;

namespace ProjectManagementAPI.Data
{
    public class AuthenticationRepository
    {
        private readonly IConfiguration _config;
        public AuthenticationRepository(IConfiguration config)
        {
            _config = config;
        }

        public async Task<UserModel> GetUserByEmail(string email)
        {
            try
            {
                using var conn = MySqlConnectionFactory.CreateConnection(_config);
                await conn.OpenAsync();

                using var cmd = new MySqlCommand("sp_getuserbyemail", conn)
                {
                    CommandType = System.Data.CommandType.StoredProcedure,
                };
                cmd.Parameters.AddWithValue("email_in", email);

                using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return new UserModel
                    {
                        Id = reader.GetInt32("id"),
                        Username = reader.GetString("username"),
                        Email = reader.GetString("email"),
                        PasswordHash = reader.GetString("passwordHash"),
                        Role = Enum.Parse<UserRole>(reader.GetString("role"), true),
                        CreatedAt = reader.GetDateTime("createdAt")
                    };
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<int?> Register(RegisterModel model)
        {
            try
            {
                using var conn = MySqlConnectionFactory.CreateConnection(_config);
                await conn.OpenAsync();

                using var cmd = new MySqlCommand("sp_register", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@username_in", model.Username);
                cmd.Parameters.AddWithValue("@email_in", model.Email);
                cmd.Parameters.AddWithValue("@password_in", PasswordHasher.ComputeSha256Hash(model.PasswordHash));
                cmd.Parameters.AddWithValue("@role_in", "admin");
                cmd.Parameters.AddWithValue("@createdAt_in", DateTime.UtcNow);

                var result = await cmd.ExecuteScalarAsync();
                return Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            }
        }

        public async Task<UserModel?> Login(LoginModel model)
        {
            try
            {
                using var conn = MySqlConnectionFactory.CreateConnection(_config);
                await conn.OpenAsync();

                using var cmd = new MySqlCommand("sp_login", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@email_in", model.Email);
                cmd.Parameters.AddWithValue("@password_in", PasswordHasher.ComputeSha256Hash(model.Password));

                using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return new UserModel
                    {
                        Id = reader.GetInt32("id"),
                        Username = reader.GetString("username"),
                        Email = reader.GetString("email"),
                        PasswordHash = reader.GetString("passwordHash"),
                        Role = Enum.Parse<UserRole>(reader.GetString("role"), true),
                        CreatedAt = reader.GetDateTime("createdAt")
                    };
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }
}
