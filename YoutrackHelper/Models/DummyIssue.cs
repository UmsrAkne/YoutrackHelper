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

        public string TemporaryComment { get; set; } = string.Empty;

        public List<Comment> Comments { get; set; } = new ();

        public List<Comment> RecentComments { get; }

        public int DisplayRecentCommentCount { get; set; }

        public string FullName { get; }

        public bool Expanded { get; set; }

        public TimeSpan WorkingDuration { get; set; }

        public TimeSpan RegisteredWorkingDuration { get; }

        public DelegateCommand ChangeVisibilityCommand { get; set; }

        public void UpdateWorkingDuration(DateTime dateTime)
        {
            throw new NotImplementedException();
        }
    }
}