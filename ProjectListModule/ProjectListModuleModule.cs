using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using ProjectListModule.Views;

namespace ProjectListModule
{
    public class ProjectListModuleModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionMan = containerProvider.Resolve<IRegionManager>();
            regionMan.RegisterViewWithRegion("ContentRegion", typeof(ViewA));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
        }
    }
}