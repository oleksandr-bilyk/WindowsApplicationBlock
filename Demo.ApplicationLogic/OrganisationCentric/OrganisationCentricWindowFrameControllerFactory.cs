using Tampleworks.WindowsApplicationBlock.Demo.ApplicationLogic.ViewDataModel;
using Tampleworks.WindowsApplicationBlock.ApplicationLogicAbstractions;

namespace Tampleworks.WindowsApplicationBlock.Demo.ApplicationLogic.OrganisationCentric
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
