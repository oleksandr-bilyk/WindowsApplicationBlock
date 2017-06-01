using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Tampleworks.WindowsApplicationBlock.ApplicationLogicAbstractions;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.Foundation.Metadata;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace Tampleworks.WindowsApplicationBlock.ApplicationLogicEnvironment
{
    /// <summary>
    /// Hnadles Application lifetime logic.
    /// </summary>
    public sealed class ApplicationManager
    {
        private readonly Func<IApplicationLogicFactory> getApplicationLogicFactory;
        private readonly Application application;
        private readonly ExtendedExecutionSessionFactory extendedExecutionManager;
        private bool isInBackgroundMode = true;
        private readonly ConcurrentQueue<TaskCompletionSource<Window>> taskWrappers = new ConcurrentQueue<TaskCompletionSource<Window>>();
        private ApplicationLogicAgent applicationLogicAgent;
        private IApplicationLogic applicaitonLogic;
        private readonly PageViewModelNavigator pageViewModelNavigator;
        private ViewManager primaryViewManager;
        private List<ViewManager> views = new List<ViewManager>();

        public ApplicationManager(
            Func<IApplicationLogicFactory> getApplicationLogicFactory,
            Application application,
            Func<Guid, Type> getPageTypeByPageId
        )
        {
            this.getApplicationLogicFactory = getApplicationLogicFactory;
            this.application = application;
            pageViewModelNavigator = new PageViewModelNavigator(getPageTypeByPageId);
            extendedExecutionManager = new ExtendedExecutionSessionFactory();

            application.Suspending += OnSuspending;
            application.Resuming += OnResument;
            if (IsBackgroundExecutionSupported)
            {
                application.EnteredBackground += App_EnteredBackground;
                application.LeavingBackground += App_LeavingBackground;
            }
        }

        private bool IsBackgroundExecutionSupported =>
            ApiInformation.IsEventPresent("Windows.UI.Xaml.Application", "EnteredBackground")
            &&
            ApiInformation.IsEventPresent("Windows.UI.Xaml.Application", "LeavingBackground");

        public async void OnLaunched(LaunchActivatedEventArgs e)
        {
            if (applicaitonLogic == null)
            {
                var applicaitonLogicFactory = getApplicationLogicFactory.Invoke();
                applicationLogicAgent = new ApplicationLogicAgent(e.Arguments, extendedExecutionManager, DisposeViewAsync);
                applicaitonLogic = applicaitonLogicFactory.GetApplicationLogic(applicationLogicAgent);
            }

            if (e.PreviousExecutionState == ApplicationExecutionState.Terminated || e.PreviousExecutionState == ApplicationExecutionState.ClosedByUser)
            {
                // TODO: Populate the UI with the previously saved application data
            }
            else
            {
                // TODO: Populate the UI with defaults
            }

            if (primaryViewManager == null)
            {
                primaryViewManager = NewViewManager(CoreApplication.MainView, applicaitonLogic.PrimaryWindowFrameControllerFactory);
                views.Add(primaryViewManager);
            }

            if (e.PrelaunchActivated == false)
            {
                await InitAllViews();
            }
        }

        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();

            applicationLogicAgent.OnSuspending();
            views.ForEach(item => item.RiseSuspending());
            deferral.Complete();
        }

        private void OnResument(object sender, object e)
        {
            applicationLogicAgent.OnResument();
            views.ForEach(item => item.RiseResument());
        }

        void App_EnteredBackground(object sender, EnteredBackgroundEventArgs e)
        {
            isInBackgroundMode = true;
            applicationLogicAgent.OnEnteredBackground();
            views.ForEach(item => item.RiseEnteredBackground());
        }

        async void App_LeavingBackground(object sender, LeavingBackgroundEventArgs e)
        {
            isInBackgroundMode = false;

            applicationLogicAgent.OnLeavingBackground();

            await InitAllViews();
            views.ForEach(item => item.RiseLeavingBackground());
        }

        internal async Task InitAllViews()
        {
            foreach (ViewManager item in views)
            {
                await item.InitContent();
            }
        }

        private ViewManager NewViewManager(CoreApplicationView coreApplicationView, IWindowFrameControllerFactory windowFrameControllerFactory)
        {
            var windowControllerContext = new ViewManager(
                coreApplicationView, pageViewModelNavigator, windowFrameControllerFactory, OpenNewViewAsync);

            windowControllerContext.Consolidated += ApplicationView_Consolidated;
            void ApplicationView_Consolidated(ApplicationView sender, ApplicationViewConsolidatedEventArgs args)
            {
                /// Important: if you set breakpoint here then UWP will close wrong window. I assume (oleksandr.bilyk@live.com) that this is strange bug.
                /// Thas's why it is better to trace this even handler.
                if (args.IsUserInitiated)
                {
                    int removedCount = views.RemoveAll(item => item.Id == sender.Id);
                    Debug.WriteLine($"Views consolidation removed {removedCount} items and leaved {views.Count}.");
                }
            }

            return windowControllerContext;
        }

        /// <summary>
        /// Reduce memory ussage by unloading View/ViewModel layers.
        /// </summary>
        public async Task DisposeViewAsync()
        {
            if (!isInBackgroundMode) throw new InvalidOperationException();

            foreach (var item in views)
            {
                await item.ClearContent();
            }
        }

        private async Task<bool> OpenNewViewAsync(IWindowFrameControllerFactory windowFrameControllerFactory)
        {
            CoreApplicationView coreApplicationView = CoreApplication.CreateNewView();

            ViewManager windowControllerContext = default(ViewManager);
            await coreApplicationView.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal,
                async () =>
                {
                    windowControllerContext = NewViewManager(coreApplicationView, windowFrameControllerFactory);
                    await windowControllerContext.InitContent();
                }
            );
            bool viewShown = await ApplicationViewSwitcher.TryShowAsStandaloneAsync(windowControllerContext.Id);
            if (viewShown)
            {
                views.Add(windowControllerContext);
            }
            else
            {
                windowControllerContext.RiseViewClosing();
                coreApplicationView.CoreWindow.Close();
            }
            return viewShown;
        }
    }
}
