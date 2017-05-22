using System;
using System.Collections.Generic;
using Tampleworks.WindowsApplicationBlock.WindowsUniversalView;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;

namespace Tampleworks.WindowsApplicationBlock.Demo.UwpApp
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        private ApplicationManager applicationManager;

        private readonly Lazy<Dictionary<Guid, Type>> pageViewMap = new Lazy<Dictionary<Guid, Type>>(GetPageViewMap);

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            Windows.ApplicationModel.Core.CoreApplication.EnablePrelaunch(true);

            if (applicationManager == null)
            {
                applicationManager = new ApplicationManager(
                    () => CompositionRoot.Instance.GetApplicationLogicFactory(),
                    this,
                    (Guid key) => pageViewMap.Value[key]
                );
            }
            applicationManager.OnLaunched(e);
        }

        private static Dictionary<Guid, Type> GetPageViewMap() => new Dictionary<Guid, Type>
        {
            { Demo.AppLogic.MainPage.MainPageViewModelFactory.PageTypeId, typeof(MainPage) },
            { Demo.AppLogic.OrganisationCentric.OrganisationCentricViewModelFactory.PageTypeId, typeof(OrganisationCentric.OrganisationCentricPageForSecondaryWindow) },
            { Demo.AppLogic.OrganisationCentric.OrganisationCentricPageViewModelFactory.PageTypeId, typeof(OrganisationCentric.OrganisationCentricPageForMainWindow) },
        };
    }
}
