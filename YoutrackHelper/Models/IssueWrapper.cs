using System;
using System.Linq;
using Newtonsoft.Json.Linq;
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

        public IssueWrapper(Issue issue)
        {
            Issue = issue;
            if (Issue != null)
            {
                ShortName = Issue.Id;
                var f = Issue.Fields.FirstOrDefault(f => f.Name == "State");
                if (f != null)
                {
                    Completed = ((JArray)f.ValueId)[0].ToString() == "完了";
                }
            }
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

        private Issue Issue { get; set; }
    }
}