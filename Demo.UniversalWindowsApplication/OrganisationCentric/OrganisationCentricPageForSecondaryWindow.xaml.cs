using Tampleworks.WindowsApplicationBlock.Demo.AppLogic.OrganisationCentric;
using Tampleworks.WindowsApplicationBlock.ApplicationLogicEnvironment;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Tampleworks.WindowsApplicationBlock.Demo.UwpApp.OrganisationCentric
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class OrganisationCentricPageForSecondaryWindow : Page
    {
        public OrganisationCentricPageForSecondaryWindow()
        {
            this.InitializeComponent();
        }

        public OrganisationCentricViewModel ViewModel { get; private set; }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var paremeter = (PageNavitedToParameters)e.Parameter;
            ViewModel = (OrganisationCentricViewModel)paremeter.ViewModel;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            ViewModel = null;
        }
    }
}
