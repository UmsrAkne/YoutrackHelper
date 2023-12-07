using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using YoutrackHelper.Models;
using YoutrackHelper.Views;
using YouTrackSharp.Projects;

namespace YoutrackHelper.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class IssueListViewViewModel : BindableBase, INavigationAware
    {
        private ObservableCollection<IIssue> issues;
        private TimeCounter timeCounter = new ();
        private bool uiEnabled;
        private string temporaryIssueTitle;
        private string temporaryIssueDescription;
        private readonly IRegionManager regionManager;
        private readonly IDialogService dialogService;

        public IssueListViewViewModel(IRegionManager regionManager, IDialogService dialogService)
        {
            this.regionManager = regionManager;
            this.dialogService = dialogService;
        }

        public Project Project { get; set; }

        public Connector Connector { get; set; }

        public ObservableCollection<IIssue> IssueWrappers { get => issues; set => SetProperty(ref issues, value); }

        public bool UiEnabled { get => uiEnabled; set => SetProperty(ref uiEnabled, value); }

        public string TemporaryIssueTitle
        {
            get => temporaryIssueTitle;
            set => SetProperty(ref temporaryIssueTitle, value);
        }

        public string TemporaryIssueDescription
        {
            get => temporaryIssueDescription;
            set => SetProperty(ref temporaryIssueDescription, value);
        }

        public DelegateCommand<IIssue> CompleteIssueCommand => new ((param) =>
        {
            if (param is IssueWrapper w)
            {
                _ = w.Complete(Connector, timeCounter);
            }
        });

        public DelegateCommand<IIssue> ChangeStatusCommand => new ((param) =>
        {
            if (param is IssueWrapper w)
            {
                _ = w.ToggleStatus(Connector, timeCounter);
            }
        });

        public DelegateCommand UpdateIssueListCommand => new (() =>
        {
            UiEnabled = false;
            _ = GetIssuesAsync();
        });

        public DelegateCommand CreateIssueCommand => new (() =>
        {
            if (string.IsNullOrWhiteSpace(TemporaryIssueTitle))
            {
                return;
            }

            _ = PostIssue(Project.ShortName, TemporaryIssueTitle, TemporaryIssueDescription);
            TemporaryIssueTitle = string.Empty;
            TemporaryIssueDescription = string.Empty;
        });

        public DelegateCommand ShowProjectListViewCommand => new (() =>
        {
            regionManager.RequestNavigate("ContentRegion", nameof(ProjectListView));
        });

        public DelegateCommand<IssueWrapper> ShowIssueDetailDialogCommand => new ((param) =>
        {
            if (param == null)
            {
                return;
            }

            var dp = new DialogParameters
            {
                { nameof(IssueWrapper), param },
                { nameof(Connector), Connector },
                { nameof(TimeCounter), timeCounter },
            };

            dialogService.ShowDialog(nameof(IssueDetailPage), dp, _ =>
            {
            });
        });

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            Project = navigationContext.Parameters.GetValue<Project>(nameof(Project));
            Connector = navigationContext.Parameters.GetValue<Connector>(nameof(Connector));
            _ = GetIssuesAsync();
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        private async Task GetIssuesAsync()
        {
            await Connector.LoadIssues(Project.ShortName);
            IssueWrappers = new ObservableCollection<IIssue>(Connector.IssueWrappers);
            UiEnabled = true;
            // Message = Connector.ErrorMessage;
        }

        private async Task PostIssue(string projectShortName, string title, string description)
        {
            UiEnabled = false;
            await Connector.CreateIssue(projectShortName, title, description);
            await GetIssuesAsync();
        }

        private async Task ApplyCommand(string shortName, string command, string comment)
        {
            var issue = await Connector.ApplyCommand(shortName, command, comment);
            var old = IssueWrappers.FirstOrDefault(i => i.ShortName == issue.Id);
            if (old != null)
            {
                var index = IssueWrappers.IndexOf(old);
                ((IssueWrapper)IssueWrappers[index]).SetIssue(issue);
            }
        }
    }
}