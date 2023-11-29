using System;
using System.Collections.Generic;
using Prism.Commands;
using YouTrackSharp.Issues;

namespace YoutrackHelper.Models
{
    public class DummyIssue : IIssue
    {
        public string Title { get; set; } = "test issue";

        public DateTime CreatedAt { get; set; }

        public bool Completed { get; set; }

        public string Status { get; set; } = "未完了";

        public string ShortName { get; set; } = string.Empty;

        public List<Comment> Comments { get; set; } = new ();

        public bool Expanded { get; set; }

        public DelegateCommand ChangeVisibilityCommand { get; set; }
    }
}