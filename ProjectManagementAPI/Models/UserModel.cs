namespace ProjectManagementAPI.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public UserRole Role { get; set; } = UserRole.viewer;
        public DateTime CreatedAt { get; set; } =  DateTime.UtcNow;
    }

    public enum UserRole
    {
        admin,
        pm,
        developer,
        viewer
    }

    public class UpdateUserRoleRequest
    {
        public string Role { get; set; } = string.Empty;
    }


    public class GetUserModel
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
