using Tampleworks.WindowsApplicationBlock.Demo.ApplicationLogic.MainPage;
using Windows.ApplicationModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Tampleworks.WindowsApplicationBlock.Demo.UniversalWindowsApplication
{
    public sealed partial class OrganisationDetails : UserControl
    {
        public OrganisationDetails()
        {
            this.InitializeComponent();
        }

        private OrganisationDetailsViewModel model;

        public OrganisationDetailsViewModel Model
        {
            get { return model; }
            set
            {
                if (model == value) return;
                model = value;
                Bindings.Update();
            }
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (model != null)
            {
                await model.ReportGeneration.ObserverReportGeneration();
            }
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Bindings.StopTracking();
        }
    }
}
