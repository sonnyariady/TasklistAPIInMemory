﻿namespace TasklistAPI.Model.Request
{
    public class TaskRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime? DueDate { get; set; }
        public int? Priority { get; set; }
    }
}
