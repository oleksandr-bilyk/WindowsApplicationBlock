﻿using Tampleworks.WindowsApplicationBlock.Demo.ApplicationLogic.OrganisationCentric;
using Tampleworks.WindowsApplicationBlock.ApplicationLogicEnvironment;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Tampleworks.WindowsApplicationBlock.Demo.UniversalWindowsApplication.OrganisationCentric
{
    public sealed partial class OrganisationCentricPageForMainWindow : Page
    {
        public OrganisationCentricPageForMainWindow()
        {
            this.InitializeComponent();
        }

        public OrganisationCentricPageViewModel ViewModel { get; private set; }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var paremeter = (PageNavitedToParameters)e.Parameter;
            ViewModel = (OrganisationCentricPageViewModel)paremeter.ViewModel;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
