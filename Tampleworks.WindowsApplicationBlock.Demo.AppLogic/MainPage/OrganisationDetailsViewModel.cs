using System;
using Tampleworks.WindowsApplicationBlock.Demo.AppLogic.ViewDataModel;
using Tampleworks.WindowsApplicationBlock.ViewModel;

namespace Tampleworks.WindowsApplicationBlock.Demo.AppLogic.MainPage
{
    public sealed class OrganisationDetailsViewModel : ViewModelBase
    {
        private bool isFavorite;

        public OrganisationDetailsViewModel(
            Guid organisationId,
            IWindowFrameControllerAgent viewAgent,
            IViewModelDataProvider viewModelDataProvider
        )
        {
            ReportGeneration = new OrganisationReportGenerationViewModel(
                organisationId, viewAgent, viewModelDataProvider.ReportGeneration
            );
        }

        public string Address => "Some Address";

        public bool LogoIsAvaialble => Logo != null;

        public byte[] Logo { get; set; }
        public bool IsFavorite
        {
            get { return isFavorite; }
            set
            {
                if (isFavorite == value) return;
                isFavorite = value;
                OnCallerPropertyChanged();
                OnIsFavoriteChanged();
            }
        }
        public event Action IsFavoriteChanged;
        private void OnIsFavoriteChanged() => IsFavoriteChanged?.Invoke();

        public OrganisationReportGenerationViewModel ReportGeneration { get; }
    }
}
