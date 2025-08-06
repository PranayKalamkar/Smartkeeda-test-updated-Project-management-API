using MySql.Data.MySqlClient;

namespace ProjectManagementAPI.Data
{
    public static class MySqlConnectionFactory
    {
        public static MySqlConnection CreateConnection(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                                    ?? throw new InvalidOperationException("MySQL connection string not found.");

            return new MySqlConnection(connectionString);
        }
    }
}
