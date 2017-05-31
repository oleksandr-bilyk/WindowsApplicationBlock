using Tampleworks.WindowsApplicationBlock.Demo.AppLogic.ViewDataModel;
using Tampleworks.WindowsApplicationBlock.ViewModel;

namespace Tampleworks.WindowsApplicationBlock.Demo.AppLogic.OrganisationCentric
{
    internal sealed class OrganisationCentricWindowFrameControllerFactory : IWindowFrameControllerFactory
    {
        private readonly OrganisationTitle organisationData;
        private readonly IViewModelDataProvider viewModelDataProvider;

        public OrganisationCentricWindowFrameControllerFactory(
            OrganisationTitle organisationData,
            IViewModelDataProvider viewModelDataProvider)

        {
            this.organisationData = organisationData;
            this.viewModelDataProvider = viewModelDataProvider;
        }

        public IWindowFrameController GetWindowFrameController(IWindowFrameControllerAgent windowFrameControllerAgent)
            => new OrganisationCentricWindowFrameController(organisationData, viewModelDataProvider, windowFrameControllerAgent);
    }
}
