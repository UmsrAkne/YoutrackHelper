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

        private BearerTokenConnection Connection { get; set; }

        private async Task LoadProjects()
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
                throw;
            }
        }
    }
}