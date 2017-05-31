using System;
using System.Threading.Tasks;
using Tampleworks.WindowsApplicationBlock.Demo.ApplicationLogic.ViewDataModel;
using Tampleworks.WindowsApplicationBlock.Demo.ApplicationLogic.MemoryPreasure;
using Tampleworks.WindowsApplicationBlock.ApplicationLogicAbstractions;

namespace Tampleworks.WindowsApplicationBlock.Demo.ApplicationLogic.MainPage
{
    public sealed class MainPageViewModel : ViewModelBase
    {
        private readonly IWindowFrameControllerAgent viewAgent;
        private readonly IViewModelDataProvider viewModelDataProvider;
        private readonly IApplicationLogicAgent applicationAgent;
        private OrganisationList organisations;
        private bool organisationsLoaded;
        private string screenTitle = "Hello World!!!";

        public MainPageViewModel(
            IWindowFrameControllerAgent viewAgent,
            IViewModelDataProvider viewModelDataProvider,
            IApplicationLogicAgent applicationAgent
        )
        {
            this.viewAgent = viewAgent;
            this.viewAgent.ViewDisposing += /* this is the way to be notified about view dispose by meomry preasure */ ViewAgent_ViewDisposing;
            this.viewModelDataProvider = viewModelDataProvider;
            this.applicationAgent = applicationAgent;
            this.applicationAgent.EnteredBackground += /* ViewModel may be notified about entering background using application logic agent */ApplicationAgent_EnteredBackground;
            MemoryPreasureCriteria = new CriteriaViewModel(this.viewModelDataProvider, viewAgent);

            Task.Run(() => LoadOrganisationsAsync(viewAgent, viewModelDataProvider));
        }

        private void ViewAgent_ViewDisposing()
        {
            /// It is important to unsubscribe from Application Logic to avoid this object memory leak.
            this.applicationAgent.EnteredBackground -= ApplicationAgent_EnteredBackground;
        }

        private void ApplicationAgent_EnteredBackground() { }

        public string ScreenTitle
        {
            get { return screenTitle; }
            set
            {
                if (screenTitle == value) return;
                screenTitle = value;
                OnCallerPropertyChanged();
            }
        }

        public OrganisationList Organisations
        {
            get { return organisations; }
            private set
            {
                if (organisations == value) return;
                organisations = value;
                OnPropertyChanged(nameof(Organisations));
            }
        }

        public bool OrganisationsLoaded
        {
            get { return organisationsLoaded; }
            private set
            {
                if (organisationsLoaded == value) return;
                organisationsLoaded = value;
                OnPropertyChanged(nameof(OrganisationsLoaded));
            }
        }

        public CriteriaViewModel MemoryPreasureCriteria { get; }

        private async Task LoadOrganisationsAsync(IWindowFrameControllerAgent viewAgent, IViewModelDataProvider viewModelDataProvider)
        {
            OrganisationList viewModel = await OrganisationList.LoadAsync(viewAgent, this.viewModelDataProvider);
            await viewAgent.RunInViewDispatcherAsync(
                () =>
                {
                    Organisations = viewModel;
                    OrganisationsLoaded = true;
                }
            );
        }
    }
}
