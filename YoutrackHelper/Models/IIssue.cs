using System;

namespace YoutrackHelper.Models
{
    public interface IIssue
    {
        public string Title { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool Completed { get; set; }

        public string Status { get; set; }
    }
}