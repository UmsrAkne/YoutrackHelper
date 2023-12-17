using Prism.Mvvm;
using YouTrackSharp.Projects;

namespace YoutrackHelper.Models
{
    public class ProjectWrapper : BindableBase, IProject
    {
        private int issueCount;
        private int incompleteIssueCount;

        public Project Project { get; set; }

        public string Name { get; set; } = string.Empty;

        public int IssueCount { get => issueCount; set => SetProperty(ref issueCount, value); }

        public int IncompleteIssueCount
        {
            get => incompleteIssueCount;
            set => SetProperty(ref incompleteIssueCount, value);
        }
    }
}