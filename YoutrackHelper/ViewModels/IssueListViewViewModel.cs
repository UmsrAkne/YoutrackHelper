using Prism.Mvvm;
using Prism.Regions;

namespace YoutrackHelper.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class IssueListViewViewModel : BindableBase, INavigationAware
    {
        public string ProjectName { get; set; }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            // Message = navigationContext.Parameters.GetValue<string>(nameof(ProjectName));
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }
    }
}