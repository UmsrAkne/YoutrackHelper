using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
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
    public class IssueListViewViewModel : BindableBase, INavigationAware, IDisposable
    {
        private readonly IRegionManager regionManager;
        private readonly IDialogService dialogService;
        private readonly Timer timer;
        private readonly TimeCounter timeCounter = new () { TotalTimeTracking = true, };
        private ObservableCollection<IIssue> issues;
        private bool uiEnabled;
        private string temporaryIssueTitle;
        private string temporaryIssueDescription;
        private bool disposed;
        private string projectName = string.Empty;
        private TimeSpan totalWorkingDuration;

        public IssueListViewViewModel(IRegionManager regionManager, IDialogService dialogService)
        {
            this.regionManager = regionManager;
            this.dialogService = dialogService;

            timer = new Timer(1000);
            timer.Elapsed += UpdateWorkingDuration;
            timer.Start();
        }

        public Project Project { get; set; }

        public string ProjectName { get => projectName; private set => SetProperty(ref projectName, value); }

        public Connector Connector { get; set; }

        public TimeSpan TotalWorkingDuration
        {
            get => totalWorkingDuration;
            set => SetProperty(ref totalWorkingDuration, value);
        }

        public ObservableCollection<IIssue> IssueWrappers
        {
            get => issues;
            private set => SetProperty(ref issues, value);
        }

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
            ProjectName = string.Empty; // プロジェクトビューに移動する際に、タイトルバーのプロジェクト名を消すためにプロパティを消去
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
            ProjectName = Project.Name;
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

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                timer.Stop();
                timer.Dispose();
            }

            disposed = true;
        }

        private async Task GetIssuesAsync()
        {
            await Connector.LoadIssues(Project.ShortName);
            IssueWrappers = new ObservableCollection<IIssue>(Connector.IssueWrappers);
            UiEnabled = true;
        }

        private async Task PostIssue(string projectShortName, string title, string description)
        {
            UiEnabled = false;
            await Connector.CreateIssue(projectShortName, title, description);
            await GetIssuesAsync();
        }

        private void UpdateWorkingDuration(object sender, ElapsedEventArgs e)
        {
            var now = DateTime.Now;
            TotalWorkingDuration = timeCounter.GetTotalWorkingDuration(now);

            var names = timeCounter.GetTrackingNames().ToList();
            if (!names.Any())
            {
                return;
            }

            IssueWrappers
                .Where(i => names.Contains(i.ShortName))
                .ToList().ForEach(i => i.UpdateWorkingDuration(now));
        }
    }
}