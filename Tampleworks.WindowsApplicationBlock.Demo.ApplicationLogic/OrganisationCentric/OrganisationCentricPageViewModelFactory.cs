using System;
using Tampleworks.WindowsApplicationBlock.Demo.AppLogic.ViewDataModel;
using Tampleworks.WindowsApplicationBlock.ViewModel;

namespace Tampleworks.WindowsApplicationBlock.Demo.AppLogic.OrganisationCentric
{
    public sealed class OrganisationCentricPageViewModelFactory : IPageViewModelFactory
    {
        private readonly OrganisationTitle organisationData;
        private readonly IViewModelDataProvider viewModelDataProvider;
        private readonly IWindowFrameControllerAgent viewAgent;

        public OrganisationCentricPageViewModelFactory(
            OrganisationTitle organisationData,
            IViewModelDataProvider viewModelDataProvider,
            IWindowFrameControllerAgent viewAgent
        )
        {
            this.organisationData = organisationData;
            this.viewModelDataProvider = viewModelDataProvider;
            this.viewAgent = viewAgent;
        }

        public static Guid PageTypeId => new Guid("40c5586e-668b-460d-a594-b8025cc8e471");

        Guid IPageViewModelFactory.PageTypeId => PageTypeId;

        object IPageViewModelFactory.GetPageViewModel() => 
            new OrganisationCentricPageViewModel(organisationData, viewModelDataProvider, viewAgent);
    }
}
