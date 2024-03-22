using System.Diagnostics;
using System.IO;
using System.Linq;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using YoutrackHelper.Models;
using YoutrackHelper.Views;

namespace YoutrackHelper.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class MainWindowViewModel : BindableBase
    {
        private readonly IRegionManager regionManager;
        private string title = "Projects";
        private bool executed;

        public MainWindowViewModel(IRegionManager regionManager)
        {
            this.regionManager = regionManager;
            Logger.WriteMessageToFile("アプリを起動しました");
        }

        // ReSharper disable once MemberCanBeMadeStatic.Global
        public string Title
        {
            get => title;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    value = "Projects";
                }

                SetProperty(ref title, value);
            }
        }

        public DelegateCommand BindTitleCommand => new (() =>
        {
            if (executed)
            {
                return;
            }

            var region = regionManager.Regions["ContentRegion"];
            region.NavigationService.Navigated += UpdateTitle;
            executed = true;
        });

        public DelegateCommand OpenLogCommand => new DelegateCommand(() =>
        {
            if (!File.Exists("log.txt"))
            {
                return;
            }

            using var ps1 = new Process();
            ps1.StartInfo.UseShellExecute = true;
            ps1.StartInfo.FileName = "log.txt";
            ps1.Start();
        });

        private void UpdateTitle(object sender, RegionNavigationEventArgs regionNavigationEventArgs)
        {
            var region = regionManager.Regions["ContentRegion"];
            var views = region.Views;

            var v = (IssueListView)views.FirstOrDefault(v => v is IssueListView);

            if (v?.DataContext is IssueListViewViewModel vm)
            {
                Title = vm.ProjectName;
            }
        }
    }
}