using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using YoutrackHelper.Models;
using YouTrackSharp.Projects;

namespace YoutrackHelper.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ProjectListViewViewModel : BindableBase
    {
        private readonly IRegionManager regionManager;
        private List<Project> projects;
        private string message = string.Empty;

        public ProjectListViewViewModel(IRegionManager regionManager)
        {
            this.regionManager = regionManager;

            var uri = File.ReadAllText(
                    $@"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\youtrackInfo\uri.txt")
                .Replace("\n", string.Empty);

            var perm = File.ReadAllText(
                    $@"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\youtrackInfo\perm.txt")
                .Replace("\n", string.Empty);

            _ = GetProjectsAsync(uri, perm);
        }

        public List<Project> Projects { get => projects; private set => SetProperty(ref projects, value); }

        public string Message { get => message; private set => SetProperty(ref message, value); }

        public DelegateCommand<Project> ShowIssueListViewCommand => new ((p) =>
        {
            if (p == null)
            {
                return;
            }

            var param = new NavigationParameters { { nameof(Project), p }, };
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