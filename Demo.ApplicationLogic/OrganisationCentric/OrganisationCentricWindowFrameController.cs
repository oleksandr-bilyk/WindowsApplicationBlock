using System;
using Tampleworks.WindowsApplicationBlock.Demo.ApplicationLogic.ViewDataModel;
using Tampleworks.WindowsApplicationBlock.ApplicationLogicAbstractions;

namespace Tampleworks.WindowsApplicationBlock.Demo.ApplicationLogic.OrganisationCentric
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

        IPageViewModelFactory IWindowFrameController.StartPageViewModelFactory => 
            new OrganisationCentricViewModelFactory(organisationData, viewModelDataProvider, viewAgent);
    }
}
