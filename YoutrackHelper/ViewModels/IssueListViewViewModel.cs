using System;
using System.Collections.ObjectModel;
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
    public class IssueListViewViewModel : BindableBase, INavigationAware
    {
        private ObservableCollection<IIssue> issues;
        private TimeCounter timeCounter = new ();

        public Project Project { get; set; }

        public Connector Connector { get; set; }

        public ObservableCollection<IIssue> IssueWrappers { get => issues; set => SetProperty(ref issues, value); }

        public DelegateCommand<IIssue> CompleteIssueCommand => new ((param) =>
        {
            _ = Connector.ApplyCommand(param.ShortName, "state 完了", string.Empty);
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
            // Message = Connector.ErrorMessage;
        }

        private async Task ApplyCommand(string shortName, string command, string comment)
        {
            var issue = await Connector.ApplyCommand(shortName, command, comment);
            var old = IssueWrappers.FirstOrDefault(i => i.ShortName == issue.ShortName);
            if (old != null)
            {
                var index = IssueWrappers.IndexOf(old);
                IssueWrappers[index] = issue;
            }
        }
    }
}