using Tampleworks.WindowsApplicationBlock.Demo.ApplicationLogic.ViewDataModel;
using Tampleworks.WindowsApplicationBlock.ApplicationLogicAbstractions;

namespace Tampleworks.WindowsApplicationBlock.Demo.ApplicationLogic.MainPage
{
    public sealed class OrganisationTitleViewModel : ViewModelBase
    {
        internal readonly OrganisationTitle Data;
        private IViewModelDataProvider viewModelDataProvider;
        private bool requireExtendedExecution;

        public OrganisationTitleViewModel(
            IWindowFrameControllerAgent viewAgent,
            OrganisationTitle item, 
            IViewModelDataProvider viewModelDataProvider
        )
        {
            this.Data = item;
            this.viewModelDataProvider = viewModelDataProvider;
            Details = new OrganisationDetailsViewModel(
                item.OrganisationId, viewAgent, viewModelDataProvider);
            Details.RequireExtendedExecutionChanged += Details_RequireExtendedExecutionChanged;
        }

        private void Details_RequireExtendedExecutionChanged()
        {
            RequireExtendedExecution = Details.RequireExtendedExecution;
        }

        public string Title => Data.Title;

        public string Notes => Data.Notes;

        public bool RequireExtendedExecution
        {
            get { return requireExtendedExecution; }
            private set
            {
                if (requireExtendedExecution == value) return;
                requireExtendedExecution = value;
                OnCallerPropertyChanged();
            }
        }

        public OrganisationDetailsViewModel Details { get; }
    }
}
