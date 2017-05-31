using Tampleworks.WindowsApplicationBlock.Demo.AppLogic.OrganisationCentric;
using Windows.UI.Xaml.Controls;

namespace Tampleworks.WindowsApplicationBlock.Demo.UwpApp.OrganisationCentric
{
    public sealed partial class OrganisationCentricContent : UserControl
    {
        private OrganisationCentricViewModel model;
        public OrganisationCentricContent()
        {
            this.InitializeComponent();
        }

        public OrganisationCentricViewModel Model
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
