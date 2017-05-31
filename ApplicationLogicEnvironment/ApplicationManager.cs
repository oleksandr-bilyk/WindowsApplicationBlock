using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Tasks;
using Tampleworks.WindowsApplicationBlock.ApplicationLogicAbstractions;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation.Metadata;
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
        private readonly Func<Guid, Type> getPageTypeByPageId;
        private readonly ExtendedExecutionSessionFactory extendedExecutionManager;
        private ApplicationLogicContext applicationLogicContext;
        private bool isInBackgroundMode = true;
        private readonly ConcurrentQueue<TaskCompletionSource<Window>> taskWrappers = new ConcurrentQueue<TaskCompletionSource<Window>>();
        /// <summary>
        /// Current applicaiton viewmodel agent used to notify and dispose view layer.
        /// </summary>
        private ViewsManager windowsManger;

        public ApplicationManager(
            Func<IApplicationLogicFactory> getApplicationLogicFactory,
            Application application,
            Func<Guid, Type> getPageTypeByPageId
        )
        {
            extendedExecutionManager = new ExtendedExecutionSessionFactory();
            this.getApplicationLogicFactory = getApplicationLogicFactory;
            this.application = application;
            this.getPageTypeByPageId = getPageTypeByPageId;

            application.Suspending += OnSuspending;
            application.Resuming += OnResument;
            HandleBackgroundLifecycle(application);
        }

        public async void OnLaunched(LaunchActivatedEventArgs e)
        {
            if (applicationLogicContext == null)
            {
                var applicaitonLogicFactory = getApplicationLogicFactory.Invoke();
                var applicationLogicAgent = new ApplicationLogicAgent(e.Arguments, extendedExecutionManager, DisposeViewAsync);
                var applicaitonLogic = applicaitonLogicFactory.GetApplicationLogic(applicationLogicAgent);
                applicationLogicContext = new ApplicationLogicContext(applicationLogicAgent, applicaitonLogic);
            }

            if (e.PreviousExecutionState == ApplicationExecutionState.Terminated || e.PreviousExecutionState == ApplicationExecutionState.ClosedByUser)
            {
                // TODO: Populate the UI with the previously saved application data
            }
            else
            {
                // TODO: Populate the UI with defaults
            }

            if (windowsManger == null)
            {
                windowsManger = new ViewsManager(applicationLogicContext.ApplicaitonLogic.MainWindowFrameControllerFactory, getPageTypeByPageId);
            }

            if (e.PrelaunchActivated == false)
            {
                await windowsManger.InitAllViews();
            }
        }

        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();

            applicationLogicContext.ApplicationLogicAgent.OnSuspending();
            windowsManger.OnSuspending();
            deferral.Complete();
        }

        private void OnResument(object sender, object e)
        {
            applicationLogicContext.ApplicationLogicAgent.OnResument();
            windowsManger.OnResument();
        }

        private void HandleBackgroundLifecycle(Application application)
        {
            if (
                ApiInformation.IsEventPresent("Windows.UI.Xaml.Application", "EnteredBackground")
                &&
                ApiInformation.IsEventPresent("Windows.UI.Xaml.Application", "LeavingBackground")
            )
            {
                application.EnteredBackground += App_EnteredBackground;
                application.LeavingBackground += App_LeavingBackground;
            }

            void App_EnteredBackground(object sender, EnteredBackgroundEventArgs e)
            {
                Debug.WriteLine("App_EnteredBackground");
                isInBackgroundMode = true;
                applicationLogicContext.ApplicationLogicAgent.OnEnteredBackground();
                windowsManger.OnEnteredBackground();
            }

            async void App_LeavingBackground(object sender, LeavingBackgroundEventArgs e)
            {
                Debug.WriteLine("App_LeavingBackground");
                isInBackgroundMode = false;

                applicationLogicContext.ApplicationLogicAgent.OnLeavingBackground();

                await windowsManger.InitAllViews();
                windowsManger.OnLeavingBackground();
            }
        }

        /// <summary>
        /// Reduce memory ussage by unloading View/ViewModel layers.
        /// </summary>
        public async Task DisposeViewAsync()
        {
            if (!isInBackgroundMode) throw new InvalidOperationException();

            await windowsManger.DisposeViewAsync();
        }
    }
}
