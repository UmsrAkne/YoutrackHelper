using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Prism.Commands;
using Prism.Mvvm;
using YouTrackSharp.Issues;

namespace YoutrackHelper.Models
{
    public class IssueWrapper : BindableBase, IIssue
    {
        private DateTime createdAt;
        private string status;
        private bool completed;
        private string shortName = string.Empty;
        private bool expanded;

        public IssueWrapper(Issue issue)
        {
            SetIssue(issue);
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
                    var tz = TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time");
                    return TimeZoneInfo
                        .ConvertTime(DateTimeOffset.FromUnixTimeMilliseconds((long)c.Value), tz)
                        .DateTime;
                }

                return createdAt;
            }
            set => createdAt = value;
        }

        public bool Completed { get => completed; set => SetProperty(ref completed, value); }

        public string Status
        {
            get
            {
                var f = Issue.Fields.FirstOrDefault(f => f.Name == "State");
                if (f != null)
                {
                    var token = ((JArray)f.ValueId)[0];
                    var str = token.ToString();
                    Completed = str == "完了";
                    return str;
                }

                return status;
            }
            set => SetProperty(ref status, value);
        }

        public string ShortName { get => shortName; set => SetProperty(ref shortName, value); }

        public List<Comment> Comments { get; set; } = new ();

        public bool Expanded { get => expanded; set => SetProperty(ref expanded, value); }

        public DelegateCommand ChangeVisibilityCommand => new (() =>
        {
            Expanded = !Expanded;
        });

        private Issue Issue { get; set; }

        public void SetIssue(Issue issue)
        {
            Issue = issue;
            if (Issue == null)
            {
                return;
            }

            ShortName = Issue.Id;
            var f = Issue.Fields.FirstOrDefault(f => f.Name == "State");
            if (f != null)
            {
                Completed = ((JArray)f.ValueId)[0].ToString() == "完了";
            }

            RaisePropertyChanged(nameof(Status));

            Comments = Issue.Comments.ToList();
        }
    }
}