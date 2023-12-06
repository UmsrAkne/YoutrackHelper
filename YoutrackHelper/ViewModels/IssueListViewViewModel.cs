using System;
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
            _ = Connector.ApplyCommand(param.ShortName, "state 完了", string.Empty);
            if (!timeCounter.IsTrackingNameRegistered(param.ShortName))
            {
                return;
            }

            var now = DateTime.Now;
            var workingDuration = timeCounter.FinishTimeTracking(param.ShortName, now);
            var startedAt = now - workingDuration;
            const string format = "yyyy/MM/dd HH:mm";
            _ = ApplyCommand(param.ShortName, "comment", $"作業完了 {startedAt.ToString(format)} - {now.ToString(format)}");

            // 取得した時間が 60秒 に満たない場合は、誤操作によるものとして登録しない。
            if (workingDuration.TotalSeconds > 60)
            {
                _ = ApplyCommand(param.ShortName, $"作業 {(int)workingDuration.TotalMinutes}m", string.Empty);
            }
        });

        public DelegateCommand<IIssue> ChangeStatusCommand => new ((param) =>
        {
            if (param.Status == "未完了")
            {
                _ = ApplyCommand(param.ShortName, "state 作業中", string.Empty);
                timeCounter.StartTimeTracking(param.ShortName, DateTime.Now);
            }

            if (param.Status == "作業中")
            {
                _ = ApplyCommand(param.ShortName, "state 未完了", string.Empty);

                if (!timeCounter.IsTrackingNameRegistered(param.ShortName))
                {
                    return;
                }

                var workingDuration = timeCounter.FinishTimeTracking(param.ShortName, DateTime.Now);

                // 取得した時間が 60秒 に満たない場合は、誤操作によるものとして登録しない。
                if (workingDuration.TotalSeconds > 60)
                {
                    _ = ApplyCommand(param.ShortName, $"作業 {(int)workingDuration.TotalMinutes}m", string.Empty);
                }
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