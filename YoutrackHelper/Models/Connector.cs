using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using YouTrackSharp;
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

        public string ErrorMessage { get; private set; } = string.Empty;

        private BearerTokenConnection Connection { get; set; }

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
    }
}