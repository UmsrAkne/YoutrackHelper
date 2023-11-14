using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Prism.Mvvm;
using ProjectListModule.Models;
using YouTrackSharp.Projects;

namespace ProjectListModule.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ViewAViewModel : BindableBase
    {
        private List<Project> projects;

        public List<Project> Projects { get => projects; private set => SetProperty(ref projects, value); }

        public ViewAViewModel()
        {
            var uri = File.ReadAllText(
                    $@"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\youtrackInfo\uri.txt")
                .Replace("\n", "");

            var perm = File.ReadAllText(
                    $@"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\youtrackInfo\perm.txt")
                .Replace("\n", "");

            _ = GetProjectsAsync(uri, perm);
        }

        private async Task GetProjectsAsync(string uri, string perm)
        {
            var connector = new Connector(uri, perm);
            await connector.LoadProjects();
            Projects = connector.Projects;
        }
    }
}