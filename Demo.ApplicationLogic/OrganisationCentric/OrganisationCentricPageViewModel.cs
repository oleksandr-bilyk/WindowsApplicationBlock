using Tampleworks.WindowsApplicationBlock.Demo.AppLogic.ViewDataModel;
using Tampleworks.WindowsApplicationBlock.ApplicationLogicAbstractions;

namespace Tampleworks.WindowsApplicationBlock.Demo.AppLogic.OrganisationCentric
{
    public sealed class OrganisationCentricPageViewModel : ViewModelBase
    {
        private readonly IWindowFrameControllerAgent viewAgent;
        public OrganisationCentricPageViewModel(
            OrganisationTitle organisationData,
            IViewModelDataProvider viewModelDataProvider,
            IWindowFrameControllerAgent viewAgent
        )
        {
            Organisation = new OrganisationCentricViewModel(organisationData, viewModelDataProvider);
            this.viewAgent = viewAgent;
        }

        public OrganisationCentricViewModel Organisation { get; }
        public void GoBack()
        {
            viewAgent.Navigation.GoBack();
        }
    }
}
