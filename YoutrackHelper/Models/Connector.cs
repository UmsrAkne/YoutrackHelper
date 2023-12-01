using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using YouTrackSharp;
using YouTrackSharp.Issues;
using YouTrackSharp.Projects;

namespace YoutrackHelper.Models
{
    public class Connector
    {
        public Connector(string url, string token)
        {
            Connection = new BearerTokenConnection(url, token);
        }

        public List<Project> Projects { get; private set; }

        public List<IIssue> IssueWrappers { get; set; }

        public string ErrorMessage { get; private set; } = string.Empty;

        private BearerTokenConnection Connection { get; set; }

        public async Task<IIssue> ApplyCommand(string shortName, string command, string comment)
        {
            var s = Connection.CreateIssuesService();
            if (string.IsNullOrWhiteSpace(comment))
            {
                await s.ApplyCommand(shortName, command);
            }
            else
            {
                await s.ApplyCommand(shortName, command, comment);
            }

            return new IssueWrapper(await s.GetIssue(shortName));
        }

        public async Task LoadProjects()
        {
            try
            {
                var projectsService = Connection.CreateProjectsService();
                var projects = await projectsService.GetAccessibleProjects();
                Projects = projects.ToList();
            }
            catch (Exception e)
            {
                Debug.WriteLine($"{e}(Connector : 46)");
                ErrorMessage = "接続に失敗しました";
            }
        }

        public async Task LoadIssues(string shortName)
        {
            try
            {
                var issuesService = Connection.CreateIssuesService();
                var issues = await issuesService.GetIssuesInProject(shortName);
                IssueWrappers = issues
                    .Select(i => (IIssue)new IssueWrapper(i))
                    .OrderBy(i => i.Completed)
                    .ThenByDescending(i => i.ShortName)
                    .ToList();
            }
            catch (Exception e)
            {
                Debug.WriteLine($"{e}(Connector : 52)");
                ErrorMessage = "接続に失敗しました";
            }
        }

        public async Task CreateIssue(string projectId, string title, string description)
        {
            try
            {
                var issuesService = Connection.CreateIssuesService();
                var issue = new Issue
                {
                    Summary = title,
                    Description = description,
                };

                await issuesService.CreateIssue(projectId, issue);
            }
            catch (Exception e)
            {
                Debug.WriteLine($"{e}(Connector : 52)");
                ErrorMessage = "接続に失敗しました";
            }
        }
    }
}