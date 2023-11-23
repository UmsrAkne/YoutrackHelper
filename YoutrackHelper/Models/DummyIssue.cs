using System;

namespace YoutrackHelper.Models
{
    public class DummyIssue : IIssue
    {
        public string Title { get; set; } = "test issue";

        public DateTime CreatedAt { get; set; }

        public bool Completed { get; set; }

        public string Status { get; set; } = "未完了";
    }
}