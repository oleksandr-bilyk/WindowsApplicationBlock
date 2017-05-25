using System;
using System.Threading.Tasks;

namespace Tampleworks.WindowsApplicationBlock.ViewModel
{
    /// <remarks>
    /// Extended exection ussage is very not trivial task because it aquire and revoke may happed asynchronously.
    /// Client developer should be able naturally write application logic that will try requires extended exection
    /// session but be aware about its possible lost.
    /// That's why we build extension that will give context that may change its extended execution state
    /// and client logic developer will be able to handle it. For example user may be notified about work is being 
    /// executed without extended exection session and be aware that application will stop exection while minimized.
    /// See MSDN artile "Run while minimized with extended execution" https://docs.microsoft.com/en-us/windows/uwp/launch-resume/run-minimized-with-extended-execution
    /// "Windows 10 Background Execution and Multi-Tasking" https://channel9.msdn.com/Events/Ignite/2015/BRK3344
    /// </remarks>
    public static class ExtendedExecutionTools
    {
        public static async Task ExecuteInOptionalExtendedExectuionContext(
            this IExtendedExecutionSessionFactory extendedExecutionSessionFactory,
            Func<ExecuteInOptionalExtendedExectuionContextArg, Task> taskFunc
        )
        {
            var context = new ExecuteInOptionalExtendedExectuionContextArg(extendedExecutionSessionFactory);
            await context.TryAcquireExtendedExecution();
            
            await taskFunc(context);
        }

        public interface IExecuteInOptionalExtendedExectuionContextArg
        {
            bool InExtendedExecution { get; }
            event Action InExtendedExecutionChanged;
            Task<bool> TryAcquireExtendedExecution();
        }

        public sealed class ExecuteInOptionalExtendedExectuionContextArg
        {
            private readonly IExtendedExecutionSessionFactory extendedExecutionSessionFactory;
            private IDisposable extendedExectuionSession;

            internal ExecuteInOptionalExtendedExectuionContextArg(
                IExtendedExecutionSessionFactory extendedExecutionSessionFactory
            )
            {
                this.extendedExecutionSessionFactory = extendedExecutionSessionFactory;
            }

            public bool InExtendedExecution { get; }
            public event Action InExtendedExecutionChanged;

            private void OnInExtendedExecutionChanged() => InExtendedExecutionChanged?.Invoke();

            public async Task<bool> TryAcquireExtendedExecution()
            {
                if (extendedExectuionSession != null) throw new InvalidOperationException();

                extendedExectuionSession = await extendedExecutionSessionFactory.TryRequestAsync(
                    "Rendering",
                    revokedBySystemPolicy: OnExtendedExectionRevoked
                );
                return extendedExectuionSession != null;
            }

            private void OnExtendedExectionRevoked()
            {
                OnInExtendedExecutionChanged();
            }
        }
    }
}
