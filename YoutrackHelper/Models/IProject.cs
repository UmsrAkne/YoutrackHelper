using YouTrackSharp.Projects;

namespace YoutrackHelper.Models
{
    public interface IProject
    {
        public Project Project { get; set; }

        public string Name { get; set; }

        public int IssueCount { get; set; }

        public int IncompleteIssueCount { get; set; }
    }
}