using System;
using System.Collections.Generic;
using Prism.Commands;
using YouTrackSharp.Issues;

namespace YoutrackHelper.Models
{
    public interface IIssue
    {
        public string Title { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool Completed { get; set; }

        public string Status { get; set; }

        public string ShortName { get; set; }

        public string TemporaryComment { get; set; }

        public List<Comment> Comments { get; set; }

        public List<Comment> RecentComments { get; }

        public int DisplayRecentCommentCount { get; set; }

        public bool Expanded { get; set; }

        public TimeSpan WorkingDuration { get; set; }

        public DelegateCommand ChangeVisibilityCommand { get; }

        public void UpdateWorkingDuration(DateTime dateTime);
    }
}