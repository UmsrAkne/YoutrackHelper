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

        public Project Project { get; set; }

        public Connector Connector { get; set; }

        public ObservableCollection<IIssue> IssueWrappers { get => issues; set => SetProperty(ref issues, value); }

        public DelegateCommand<IIssue> CompleteIssueCommand => new ((param) =>
        {
            _ = Connector.ApplyCommand(param.ShortName, "state 完了", string.Empty);
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
    }
}