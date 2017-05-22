using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Tampleworks.WindowsApplicationBlock.Demo.AppLogic.ViewDataModel;
using Tampleworks.WindowsApplicationBlock.ViewModel;

namespace Tampleworks.WindowsApplicationBlock.Demo.AppLogic.MainPage
{
    public sealed class OrganisationList : ViewModelBase
    {
        private readonly IWindowFrameControllerAgent viewAgent;
        private readonly IViewModelDataProvider viewModelDataProvider;
        private OrganisationTitleViewModel itemSelected;

        public OrganisationList(
            IWindowFrameControllerAgent viewAgent,
            List<OrganisationTitle> organisationList,
            IViewModelDataProvider viewModelDataProvider
        )
        {
            this.viewAgent = viewAgent;
            this.viewModelDataProvider = viewModelDataProvider;
            this.ItemCollection = (from item in organisationList select new OrganisationTitleViewModel(item, viewModelDataProvider)).ToList();
            itemSelected = ItemCollection.FirstOrDefault();
        }

        public List<OrganisationTitleViewModel> ItemCollection { get; }

        public OrganisationTitleViewModel ItemSelected
        {
            get { return itemSelected; }
            set
            {
                if (itemSelected == value) return;
                Debug.Assert(value == null || ItemCollection.Contains(value));
                itemSelected = value;
                OnCallerPropertyChanged();
            }
        }

        internal static async Task<OrganisationList> LoadAsync(
            IWindowFrameControllerAgent viewAgent, 
            IViewModelDataProvider viewModelDataProvider
        )
        {
            var data = await viewModelDataProvider.GetOrganisationTitleListAsync();
            return new OrganisationList(viewAgent, data, viewModelDataProvider);
        }

        public void NavigateToOrganisation()
        {
            if (itemSelected == null) return;

            var organisationData = ItemSelected.Data;

            bool opened = viewAgent.Navigation.Navigate(
                new OrganisationCentric.OrganisationCentricPageViewModelFactory(
                    organisationData,
                    viewModelDataProvider,
                    viewAgent
                )
            );
        }

        public async void OpenOrganisationInSeparateWindow()
        {
            if (itemSelected == null) return;

            var organisationData = ItemSelected.Data;

            bool opened = await viewAgent.OpenNewViewAsync(
                new OrganisationCentric.OrganisationCentricWindowFrameControllerFactory(
                    organisationData,
                    viewModelDataProvider
                )
            );
        }
    }
}
