using Newtonsoft.Json;

namespace API.Models
{
    /// <summary>
    /// Represents a model for a task.
    /// </summary>
    public class TaskItem
    {
        /// <summary>
        /// The unique ID for the record set
        /// </summary>
        [JsonProperty("id")] // Cosmos DB requires lowercase "id"
        public Guid Id           { get; set; } = new Guid();

        /// <summary>
        /// The name of the task
        /// </summary>
        public string TaskName   { get; set; }

        /// <summary>
        /// The task assignee
        /// </summary>
        public string Assignee   { get; set; }

        /// <summary>
        /// The deadline for the task
        /// </summary>
        public DateTime Deadline { get; set; }
    }
}
