using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Tampleworks.WindowsApplicationBlock.Demo.AppLogic.ApplicationLogicData;
using Tampleworks.WindowsApplicationBlock.Demo.AppLogic.Tracing;
using Tampleworks.WindowsApplicationBlock.Demo.AppLogic.ViewDataModel;
using Tampleworks.WindowsApplicationBlock.ApplicationLogicAbstractions;

namespace Tampleworks.WindowsApplicationBlock.Demo.AppLogic
{
    public sealed class ApplicationLogic : IApplicationLogic, IWindowFrameControllerFactory
    {
        private readonly IApplicationLogicAgent applicationAgent;
        private readonly IApplicationLogicDataProvider applicationLogicDataProvider;
        private bool isInBackground = true;
        private readonly MemoryController memoryController = new MemoryController();
        private readonly IApplicationMemoryManager memoryManager;
        private readonly ISemanticLogger logger;
        private readonly ExtendedExecutionTaskAgrigation extendedExecutionTaskAgrigation;

        internal ApplicationLogic(
            IApplicationLogicAgent applicationAgent, 
            IApplicationLogicDataProviderFactory applicationLogicDataProvider,
            IApplicationMemoryManager memoryManager,
            ISemanticLogger logger
        )
        {
            this.applicationAgent = applicationAgent;
            this.applicationLogicDataProvider = applicationLogicDataProvider.Get(applicationAgent, memoryController);
            this.memoryManager = memoryManager;
            this.logger = logger;
            memoryManager.AppMemoryUsageLimitChanging += MemoryManager_AppMemoryUsageLimitChanging;
            applicationAgent.EnteredBackground += () => isInBackground = true;
            applicationAgent.LeavingBackground += () => isInBackground = false;
            applicationAgent.Suspension += () => { };
            memoryController.AskApplicationToReleaseMemoryFromView += MemoryController_AskApplicationToReleaseMemoryFromView;
            extendedExecutionTaskAgrigation = new ExtendedExecutionTaskAgrigation(
                applicationAgent.ExtendedExecutionSessionFactory, "Organisation data analysis.");
        }

        private async void MemoryManager_AppMemoryUsageLimitChanging(object sender, AppMemoryUsageLimitChangingEventArgs e)
        {
            /// Read MSDN article "Free memory when your app moves to the background"
            /// https://docs.microsoft.com/en-us/windows/uwp/launch-resume/reduce-memory-usage
            /// about UWP application memory ussage
            if (isInBackground && e.NewLimit < memoryManager.AppMemoryUsage)
            {
                await TryToDisposeViewAsync();
            }
            var currentUssage = memoryManager.AppMemoryUsage;
            if (e.NewLimit < currentUssage)
            {
                memoryController.Unallocate(currentUssage - e.NewLimit);
            }
        }

        private async void MemoryController_AskApplicationToReleaseMemoryFromView()
        {
            await TryToDisposeViewAsync();
        }

        private async Task TryToDisposeViewAsync()
        {
            /// Read MSDN article "Free memory when your app moves to the background"
            /// https://docs.microsoft.com/en-us/windows/uwp/launch-resume/reduce-memory-usage
            /// about UWP application memory ussage
            if (isInBackground)
            {
                var memoryBeforeRelease = memoryManager.AppMemoryUsage;
                /// In this case when memory limit reached first of all we release View/ViewModel
                /// layers and only then release memory preasure.
                await applicationAgent.DisposeViewAsync();
                GC.Collect();
                GC.WaitForPendingFinalizers();
                await Task.Delay(1000);// Wait untill window thread will release 
                var memoryAfterRelease = memoryManager.AppMemoryUsage;
                var released = Convert.ToInt64(memoryBeforeRelease) - Convert.ToInt64(memoryAfterRelease);
                Debug.WriteLine($"View/ViewModel layers were release with {released} bytes memory.");
            }
        }

        IWindowFrameControllerFactory IApplicationLogic.MainWindowFrameControllerFactory => this;

        public IWindowFrameController GetWindowFrameController(IWindowFrameControllerAgent agent)
            =>  new ApplicationMainWindowFrameController(
                agent, 
                new ViewModelDataProvider(memoryController, extendedExecutionTaskAgrigation),
                applicationAgent
            );
    }
}
