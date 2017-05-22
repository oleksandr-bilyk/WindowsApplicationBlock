using System;
using Tampleworks.WindowsApplicationBlock.Demo.AppLogic.ViewDataModel;
using Tampleworks.WindowsApplicationBlock.ViewModel;

namespace Tampleworks.WindowsApplicationBlock.Demo.AppLogic.MainPage
{
    public class MainPageViewModelFactory : IPageViewModelFactory
    {
        private readonly IWindowFrameControllerAgent viewAgent;
        private readonly IViewModelDataProvider viewModelDataProvider;
        private readonly IApplicationLogicAgent applicationAgent;

        internal MainPageViewModelFactory(
            IWindowFrameControllerAgent viewAgent,
            IViewModelDataProvider viewModelDataProvider,
            IApplicationLogicAgent applicationAgent
        )
        {
            this.viewAgent = viewAgent;
            this.viewModelDataProvider = viewModelDataProvider;
            this.applicationAgent = applicationAgent;
        }

        Guid IPageViewModelFactory.PageTypeId => PageTypeId;
        public static Guid PageTypeId => new Guid("8881f201-3eda-4a33-a26e-0878ce0e421d");

        public object GetPageViewModel() => new MainPageViewModel(viewAgent, viewModelDataProvider, applicationAgent);
    }
}
