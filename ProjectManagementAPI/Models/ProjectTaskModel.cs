namespace ProjectManagementAPI.Models
{
    public class ProjectTaskModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int ProjectId { get; set; }
        public int AssignedTo { get; set; }
        public TaskModelStatus Status { get; set; } = TaskModelStatus.todo;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? DueDate { get; set; }

    }

    public enum TaskModelStatus
    {
        todo,
        inprogress,
        completed
    }

    public class GetProjectTaskModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public int ProjectId { get; set; }
        public string? ProjectName { get; set; } // optional
        public int AssignedTo { get; set; }
        public string? AssignedToName { get; set; } // optional
        public TaskModelStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? DueDate { get; set; }

        public List<GetUserModel>? ProjectDevelopers { get; set; } = new(); // optional
    }
}
