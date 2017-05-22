using Tampleworks.WindowsApplicationBlock.Demo.AppLogic.ViewDataModel;
using Tampleworks.WindowsApplicationBlock.ViewModel;

namespace Tampleworks.WindowsApplicationBlock.Demo.AppLogic.MainPage
{
    public sealed class OrganisationTitleViewModel : ViewModelBase
    {
        internal readonly OrganisationTitle Data;
        private IViewModelDataProvider viewModelDataProvider;
        private bool isFavorite;

        public OrganisationTitleViewModel(OrganisationTitle item, IViewModelDataProvider viewModelDataProvider)
        {
            this.Data = item;
            this.viewModelDataProvider = viewModelDataProvider;
            Details = new OrganisationDetailsViewModel();
            Details.IsFavoriteChanged += Details_IsFavoriteChanged;
        }

        private void Details_IsFavoriteChanged()
        {
            IsFavorite = Details.IsFavorite;
        }

        public string Title => Data.Title;

        public string Notes => Data.Notes;

        public bool IsFavorite
        {
            get { return isFavorite; }
            private set
            {
                if (isFavorite == value) return;
                isFavorite = value;
                OnCallerPropertyChanged();
            }
        }

        public OrganisationDetailsViewModel Details { get; }
    }
}
