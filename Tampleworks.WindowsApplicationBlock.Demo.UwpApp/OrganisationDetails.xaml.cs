using Tampleworks.WindowsApplicationBlock.Demo.AppLogic.MainPage;
using Windows.ApplicationModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Tampleworks.WindowsApplicationBlock.Demo.UwpApp
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
    }
}
