namespace ProjectManagementAPI.Models
{
    public class ProjectModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int ProjectManagerId { get; set; }
        public List<int> DeveloperIds { get; set; } = new();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ProjectStatus Status { get; set; } = ProjectStatus.pending;

    }

    public enum ProjectStatus
    {
        pending,
        inprogress,
        completed
    }

    public class GetProjectModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public int ProjectManagerId { get; set; }
        public string? ProjectManagerName { get; set; }  // NEW
        public List<GetUserModel>? Developers { get; set; } = new();  // NEW
        public ProjectStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
