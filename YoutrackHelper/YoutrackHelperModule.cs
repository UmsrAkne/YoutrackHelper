using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using YoutrackHelper.Views;

namespace YoutrackHelper
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class YoutrackHelperModule : IModule
    {
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionMan = containerProvider.Resolve<IRegionManager>();
            regionMan.RegisterViewWithRegion("ContentRegion", typeof(ProjectListView));
        }
    }
}