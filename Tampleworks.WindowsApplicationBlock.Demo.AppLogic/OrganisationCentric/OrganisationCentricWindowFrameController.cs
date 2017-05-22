using System;
using Tampleworks.WindowsApplicationBlock.Demo.AppLogic.ViewDataModel;
using Tampleworks.WindowsApplicationBlock.ViewModel;

namespace Tampleworks.WindowsApplicationBlock.Demo.AppLogic.OrganisationCentric
{
    public sealed class OrganisationCentricWindowFrameController : IWindowFrameController
    {
        private readonly OrganisationTitle organisationData;
        private readonly IViewModelDataProvider viewModelDataProvider;
        private readonly IWindowFrameControllerAgent viewAgent;

        public OrganisationCentricWindowFrameController(
            OrganisationTitle organisationData,
            IViewModelDataProvider viewModelDataProvider, 
            IWindowFrameControllerAgent viewAgent
        )
        {
            this.organisationData = organisationData;
            this.viewModelDataProvider = viewModelDataProvider;
            this.viewAgent = viewAgent;
        }

        IPageViewModelFactory IWindowFrameController.GetPageViewModelFactory() => 
            new OrganisationCentricViewModelFactory(organisationData, viewModelDataProvider, viewAgent);
    }
}
