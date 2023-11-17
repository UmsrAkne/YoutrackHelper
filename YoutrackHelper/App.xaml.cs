using System.Windows;
using Prism.Ioc;
using Prism.Modularity;
using ProjectListModule;
using ProjectListModule.ViewModels;
using ProjectListModule.Views;
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
            containerRegistry.RegisterForNavigation<ViewA, ViewAViewModel>();
            containerRegistry.RegisterForNavigation<IssueListView, IssueListViewViewModel>();
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule<ProjectListModuleModule>();
            moduleCatalog.AddModule<YoutrackHelperModule>();
        }
    }
}