using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tampleworks.WindowsApplicationBlock.ApplicationLogicAbstractions;

namespace Tampleworks.WindowsApplicationBlock.Demo.ApplicationLogic
{
    /// <summary>
    /// Executes few tasks under shared requested extended execution.
    /// </summary>
    /// <remarks>
    /// Extended exection ussage is very not trivial task because it aquire and revoke may happed asynchronously.
    /// Client developer should be able naturally write application logic that will try requires extended exection
    /// session but be aware about its possible lost.
    /// That's why we build extension that will give context that may change its extended execution state
    /// and client logic developer will be able to handle it. For example user may be notified about work is being 
    /// executed without extended exection session and be aware that application will stop exection while minimized.
    /// See MSDN artile "Run while minimized with extended execution" https://docs.microsoft.com/en-us/windows/uwp/launch-resume/run-minimized-with-extended-execution
    /// "Windows 10 Background Execution and Multi-Tasking" https://channel9.msdn.com/Events/Ignite/2015/BRK3344
    /// 
    /// On start this method attemts to request extended execution session if it was not got by other tasks
    /// then single instande of extended execution session will be created. If all tasks are completed then 
    /// extended execution session will be released.
    /// Every execution task may subscribe to extended exectuion state changed. Task may be executed without 
    /// extended execution in following cases:
    /// - extended execution cannot be aquired. Task may do not start execution at all if extended execution is required.
    /// - extended execution was revoked. Taks may urgently stop its execution if it doesn't want to be suspended application.
    /// Some tasks may request but not require extended execution then they will be accasionally frizzed till application
    /// will be resumed. 
    /// UI may notify user about extended execution session availability change.
    /// </remarks>
    public sealed class ExtendedExecutionTaskAgrigation
    {
        private readonly IExtendedExecutionSessionFactory extendedExecutionSessionFactory;
        private readonly string sessionDeskription;
        internal readonly SemaphoreSlim syncRoot = new SemaphoreSlim(1);
        private IDisposable extendedExecutionSession;
        private readonly List<ExtendedExecutionTaskAgrigationArg> parallelExecutions = new List<ExtendedExecutionTaskAgrigationArg>();

        public ExtendedExecutionTaskAgrigation(
            IExtendedExecutionSessionFactory extendedExecutionSessionFactory,
            string sessionDeskription
        )
        {
            this.extendedExecutionSessionFactory = extendedExecutionSessionFactory;
            this.sessionDeskription = sessionDeskription;
        }

        public async Task ExecuteAsync(
            Func<IExtendedExecutionTaskAgrigationArg, Task> taskFunc
        )
        {
            await ExecuteAsync<object>(
                async (arg) =>
                {
                    await taskFunc(arg);
                    return null;
                }
            );
        }

        public async Task<TResult> ExecuteAsync<TResult>(
            Func<IExtendedExecutionTaskAgrigationArg, Task<TResult>> taskFunc
        )
        {
            var arg = new ExtendedExecutionTaskAgrigationArg(this);
            try
            {// Try get extended exectuion session
                await syncRoot.WaitAsync();
                
                if (extendedExecutionSession == null)
                {
                    extendedExecutionSession = await extendedExecutionSessionFactory.TryRequestAsync(sessionDeskription, Revoke);
                }
                parallelExecutions.Add(arg);
                UpdateAllTasksInExtendedExecution();
            }
            finally { syncRoot.Release(); }

            try
            {// Strategy execution
                TResult result = await taskFunc(arg);
                return result;
            }
            finally
            {
                try
                {// release extended exectuion session
                    await syncRoot.WaitAsync();

                    parallelExecutions.Remove(arg);

                    if (parallelExecutions.Count == 0 && extendedExecutionSession != null)
                    {
                        extendedExecutionSession.Dispose();
                        extendedExecutionSession = null;
                    }
                    UpdateAllTasksInExtendedExecution();
                }
                finally { syncRoot.Release(); }
            }
        }

        private void UpdateAllTasksInExtendedExecution()
        {
            foreach (var item in parallelExecutions)
            {
                item.InExtendedExecution = extendedExecutionSession != null;
            }
        }

        private void Revoke()
        {
            try
            {
                syncRoot.Wait();

                if (extendedExecutionSession != null)
                {
                    extendedExecutionSession.Dispose();
                    extendedExecutionSession = null;
                }
                UpdateAllTasksInExtendedExecution();
            }
            finally { syncRoot.Release(); }
        }

        public interface IExtendedExecutionTaskAgrigationArg
        {
            bool InExtendedExecution { get; }
            event Action<bool> InExtendedExecutionChanged;
        }

        public sealed class ExtendedExecutionTaskAgrigationArg : IExtendedExecutionTaskAgrigationArg
        {
            private readonly ExtendedExecutionTaskAgrigation context;
            private bool inExtendedExecution;

            internal ExtendedExecutionTaskAgrigationArg(
                ExtendedExecutionTaskAgrigation context
            )
            {
                this.context = context;
            }

            public bool InExtendedExecution
            {
                get { return inExtendedExecution; }
                internal set
                {
                    if (inExtendedExecution == value) return;
                    inExtendedExecution = value;
                    OnInExtendedExecutionChanged(value);
                }
            }
            public event Action<bool> InExtendedExecutionChanged;

            private void OnInExtendedExecutionChanged(bool inExtendedExecution) 
                => InExtendedExecutionChanged?.Invoke(inExtendedExecution);
        }
    }
}
