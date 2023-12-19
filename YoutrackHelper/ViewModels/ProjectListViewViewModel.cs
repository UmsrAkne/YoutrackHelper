using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using YoutrackHelper.Models;
using YouTrackSharp.Projects;

namespace YoutrackHelper.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ProjectListViewViewModel : BindableBase, INavigationAware
    {
        private readonly IRegionManager regionManager;
        private List<IProject> projects;
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

        public List<IProject> Projects { get => projects; private set => SetProperty(ref projects, value); }

        public Connector Connector { get; set; }

        public string Message { get => message; private set => SetProperty(ref message, value); }

        public DelegateCommand<IProject> ShowIssueListViewCommand => new ((p) =>
        {
            if (p == null)
            {
                return;
            }

            var param = new NavigationParameters
            {
                { nameof(Project), p.Project },
                { nameof(Connector), Connector },
            };

            regionManager.RequestNavigate("ContentRegion", "IssueListView", param);
        });

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        private async Task GetProjectsAsync(string uri, string perm)
        {
            Connector = new Connector(uri, perm);
            await Connector.LoadProjects();
            var ps = Connector.Projects
                .Select(p => (IProject)new ProjectWrapper() { Project = p, Name = p.Name, })
                .ToList();

            Projects = ps;

            foreach (var p in ps)
            {
                var issueList = await Connector.GetIssues(p.Name);
                p.IssueCount = issueList.Count;
                p.IncompleteIssueCount = issueList.Count(i => !new IssueWrapper(i).Completed);
            }

            Message = Connector.ErrorMessage;
        }
    }
}