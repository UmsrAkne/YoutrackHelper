using System.Windows;
using Prism.Ioc;
using Prism.Modularity;
using YoutrackHelper.Models;
using YoutrackHelper.ViewModels;
using YoutrackHelper.Views;

namespace YoutrackHelper
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<ProjectListView, ProjectListViewViewModel>();
            containerRegistry.RegisterForNavigation<IssueListView, IssueListViewViewModel>();
            containerRegistry.RegisterDialog<IssueDetailPage, IssueDetailPageViewModel>();
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule<YoutrackHelperModule>();
        }
        
        protected override void OnExit(ExitEventArgs e)
        {
            Logger.WriteMessageToFile("アプリが終了しました");
            
            // アプリが終了するときの処理
            base.OnExit(e);
        }
    }
}