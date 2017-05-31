using Tampleworks.WindowsApplicationBlock.Demo.ApplicationLogic.ViewDataModel;
using Tampleworks.WindowsApplicationBlock.Demo.ApplicationLogic.MainPage;
using Tampleworks.WindowsApplicationBlock.ApplicationLogicAbstractions;

namespace Tampleworks.WindowsApplicationBlock.Demo.ApplicationLogic
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
