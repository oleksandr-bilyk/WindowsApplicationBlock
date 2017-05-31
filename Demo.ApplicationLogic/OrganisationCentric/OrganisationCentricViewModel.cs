using Tampleworks.WindowsApplicationBlock.Demo.ApplicationLogic.ViewDataModel;
using Tampleworks.WindowsApplicationBlock.ApplicationLogicAbstractions;

namespace Tampleworks.WindowsApplicationBlock.Demo.ApplicationLogic.OrganisationCentric
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
