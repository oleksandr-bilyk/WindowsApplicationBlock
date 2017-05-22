using Tampleworks.WindowsApplicationBlock.Demo.AppLogic.ViewDataModel;
using Tampleworks.WindowsApplicationBlock.Demo.AppLogic.MainPage;
using Tampleworks.WindowsApplicationBlock.ViewModel;

namespace Tampleworks.WindowsApplicationBlock.Demo.AppLogic
{
    public sealed class ApplicationMainWindowFrameController : IWindowFrameController
    {
        private readonly IWindowFrameControllerAgent viewAgent;
        private readonly IViewModelDataProvider viewModelDataProvider;
        private readonly IApplicationLogicAgent applicationAgent;

        internal ApplicationMainWindowFrameController(
            IWindowFrameControllerAgent viewAgent,
            IViewModelDataProvider viewModelDataProvider,
            IApplicationLogicAgent applicationAgent
        )
        {
            this.viewAgent = viewAgent;
            this.viewModelDataProvider = viewModelDataProvider;
            this.applicationAgent = applicationAgent;
        }

        public IPageViewModelFactory GetPageViewModelFactory() => new MainPageViewModelFactory(viewAgent, viewModelDataProvider, applicationAgent);
    }
}
