using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using ProjectListModule.Models;
using YouTrackSharp.Projects;

namespace ProjectListModule.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ProjectListViewViewModel : BindableBase
    {
        private List<Project> projects;
        private string message = string.Empty;
        private readonly IRegionManager regionManager;

        public List<Project> Projects { get => projects; private set => SetProperty(ref projects, value); }

        public string Message { get => message; private set => SetProperty(ref message, value); }

        public ProjectListViewViewModel(IRegionManager regionManager)
        {
            this.regionManager = regionManager;

            var uri = File.ReadAllText(
                    $@"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\youtrackInfo\uri.txt")
                .Replace("\n", "");

            var perm = File.ReadAllText(
                    $@"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\youtrackInfo\perm.txt")
                .Replace("\n", "");

            _ = GetProjectsAsync(uri, perm);
        }

        public DelegateCommand ShowIssueListViewCommand => new (() =>
        {
            var param = new NavigationParameters { { "ProjectName", "testLabel" }, };
            regionManager.RequestNavigate("ContentRegion", "IssueListView", param);
        });

        private async Task GetProjectsAsync(string uri, string perm)
        {
            var connector = new Connector(uri, perm);
            await connector.LoadProjects();
            Projects = connector.Projects;
            Message = connector.ErrorMessage;
        }
    }
}