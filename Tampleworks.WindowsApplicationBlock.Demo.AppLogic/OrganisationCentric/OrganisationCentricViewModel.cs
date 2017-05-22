using Tampleworks.WindowsApplicationBlock.Demo.AppLogic.ViewDataModel;
using Tampleworks.WindowsApplicationBlock.ViewModel;

namespace Tampleworks.WindowsApplicationBlock.Demo.AppLogic.OrganisationCentric
{
    public sealed class OrganisationCentricViewModel : ViewModelBase
    {
        private readonly OrganisationTitle organisationData;
        private readonly IViewModelDataProvider viewModelDataProvider;

        public OrganisationCentricViewModel(
            OrganisationTitle organisationData,
            IViewModelDataProvider viewModelDataProvider
        )
        {
            this.organisationData = organisationData;
            this.viewModelDataProvider = viewModelDataProvider;
        }

        public string Title => organisationData.Title;
        public string Notes => organisationData.Notes;

        /// <summary>
        /// This method is only for demonstration purposes how to use ViewModel layer data provider from secondary window.
        /// </summary>
        private object GetAdditionalInformationByOrganisation() => 
            viewModelDataProvider.GetOrganisationDetailsAsync(organisationData.OrganisationId);
    }
}
