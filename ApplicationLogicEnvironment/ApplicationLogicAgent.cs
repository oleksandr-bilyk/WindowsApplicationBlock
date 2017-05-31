using System;
using System.Threading.Tasks;
using Tampleworks.WindowsApplicationBlock.ApplicationLogicAbstractions;

namespace Tampleworks.WindowsApplicationBlock.ApplicationLogicEnvironment
{
    internal sealed class ApplicationLogicAgent : IApplicationLogicAgent
    {
        private readonly Func<Task> disposeViewAsync;

        public ApplicationLogicAgent(
            string arguments,
            ExtendedExecutionSessionFactory extendedExecutionManager,
            Func<Task> disposeViewAsync
        )
        {
            Arguments = arguments;
            ExtendedExecutionSessionFactory = extendedExecutionManager;
            this.disposeViewAsync = disposeViewAsync;
        }

        public string Arguments { get; }
        public IExtendedExecutionSessionFactory ExtendedExecutionSessionFactory { get; }

        public event Action EnteredBackground;
        public event Action LeavingBackground;
        public event Action Suspension;
        public event Action Resument;
        public event Action AppMemoryUsageLevelUpdated;

        internal void OnEnteredBackground() => EnteredBackground?.Invoke();
        internal void OnLeavingBackground() => LeavingBackground?.Invoke();
        internal void OnSuspending() => Suspension?.Invoke();
        internal void OnResument() => Resument?.Invoke();
        internal void OnAppMemoryUsageLevelUpdated() => AppMemoryUsageLevelUpdated?.Invoke();

        public Task DisposeViewAsync() => disposeViewAsync();
    }
}
