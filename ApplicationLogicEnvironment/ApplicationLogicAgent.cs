using System;
using System.Threading.Tasks;
using Tampleworks.WindowsApplicationBlock.ApplicationLogicAbstractions;

namespace Tampleworks.WindowsApplicationBlock.ApplicationLogicEnvironment
{
    internal sealed class ApplicationLogicAgent : IApplicationLogicAgent
    {
        private readonly Func<Task> resetViewAsync;
        private readonly OpenNewViewAsyncHandler openNewViewAsync;

        public ApplicationLogicAgent(
            string arguments,
            ExtendedExecutionSessionFactory extendedExecutionManager,
            Func<Task> disposeViewAsync,
            OpenNewViewAsyncHandler openNewViewAsync,
            IApplicationLifecycleAgent lifecycle
        )
        {
            Arguments = arguments;
            ExtendedExecutionSessionFactory = extendedExecutionManager;
            this.resetViewAsync = disposeViewAsync;
            this.openNewViewAsync = openNewViewAsync;
            Lifecycle = lifecycle;
        }

        public string Arguments { get; }
        public IExtendedExecutionSessionFactory ExtendedExecutionSessionFactory { get; }
        public IApplicationLifecycleAgent Lifecycle { get; }

        public Task ResetViewAsync() => resetViewAsync();
        public async Task<bool> OpenNewSecondaryViewAsync(IWindowFrameControllerFactory windowFrameControllerFactory) =>
            await openNewViewAsync(windowFrameControllerFactory);
    }
}
