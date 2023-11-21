using System;

namespace YoutrackHelper.Models
{
    public class IssueWrapper : IIssue
    {
        public string Title { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public bool Completed { get; set; }

        public string Status { get; set; } = string.Empty;
    }
}