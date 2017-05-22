using System;
using Tampleworks.WindowsApplicationBlock.Demo.AppLogic.ViewDataModel;
using Tampleworks.WindowsApplicationBlock.ViewModel;

namespace Tampleworks.WindowsApplicationBlock.Demo.AppLogic.OrganisationCentric
{
    public sealed class OrganisationCentricViewModelFactory : IPageViewModelFactory
    {
        private readonly OrganisationTitle organisationData;
        private readonly IViewModelDataProvider viewModelDataProvider;
        private readonly IWindowFrameControllerAgent viewAgent;

        public OrganisationCentricViewModelFactory(
            OrganisationTitle organisationData,
            IViewModelDataProvider viewModelDataProvider,
            IWindowFrameControllerAgent viewAgent
        )
        {
            this.organisationData = organisationData;
            this.viewModelDataProvider = viewModelDataProvider;
            this.viewAgent = viewAgent;
        }

        public static Guid PageTypeId => new Guid("a500de39-62df-41a1-a1ae-0434050bc877");

        Guid IPageViewModelFactory.PageTypeId => PageTypeId;

        object IPageViewModelFactory.GetPageViewModel() => new OrganisationCentricViewModel(organisationData, viewModelDataProvider);
    }
}
