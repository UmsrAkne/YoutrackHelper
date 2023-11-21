using System;
using System.Linq;
using Prism.Mvvm;
using YouTrackSharp.Issues;

namespace YoutrackHelper.Models
{
    public class IssueWrapper : BindableBase, IIssue
    {
        private DateTime createdAt;

        public IssueWrapper(Issue issue)
        {
            Issue = issue;
        }

        public string Title
        {
            get => Issue.Summary;
            set
            {
                RaisePropertyChanged();
                Issue.Summary = value;
            }
        }

        public DateTime CreatedAt
        {
            get
            {
                var c = Issue.Fields.FirstOrDefault(f => f.Name == "created");
                if (c != null)
                {
                    return new DateTime((long)c.Value);
                }

                return createdAt;
            }
            set => createdAt = value;
        }

        public bool Completed { get; set; }

        public string Status { get; set; }

        private Issue Issue { get; set; }
    }
}