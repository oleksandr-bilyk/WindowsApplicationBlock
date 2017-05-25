using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tampleworks.WindowsApplicationBlock.ViewModel;

namespace Tampleworks.WindowsApplicationBlock.Demo.AppLogic
{
    /// <summary>
    /// Executes few tasks under shared requested but not required extended execution.
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
    /// </remarks>
    public sealed class ExtendedExecutionTaskAgrigation
    {
        private readonly IExtendedExecutionSessionFactory extendedExecutionSessionFactory;
        private readonly string sessionDeskription;
        internal readonly SpinLock syncRoot = new SpinLock(enableThreadOwnerTracking: false);
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

        /// <remarks>
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
        public async Task ExecuteAsync(
            Func<IExtendedExecutionTaskAgrigationArg, Task> taskFunc
        )
        {
            var arg = new ExtendedExecutionTaskAgrigationArg(this);
            try
            {
                bool lockTaken = false;
                syncRoot.Enter(ref lockTaken);
                if (!lockTaken) throw new InvalidOperationException("Cannot taken lock.");

                parallelExecutions.Add(arg);
                if (extendedExecutionSession == null)
                {
                    extendedExecutionSession = await extendedExecutionSessionFactory.TryRequestAsync(sessionDeskription, Revoke);
                }
                UpdateTasks();
            }
            finally
            {
                syncRoot.Exit();
            }

            await taskFunc(arg);

            try
            {
                bool lockTaken = false;
                syncRoot.Enter(ref lockTaken);
                if (!lockTaken) throw new InvalidOperationException("Cannot taken lock.");

                parallelExecutions.Remove(arg);

                if (parallelExecutions.Count == 0 && extendedExecutionSession != null)
                {
                    extendedExecutionSession.Dispose();
                    extendedExecutionSession = null;
                }
                UpdateTasks();
            }
            finally
            {
                syncRoot.Exit();
            }
        }

        private void UpdateTasks()
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
                bool lockTaken = false;
                syncRoot.Enter(ref lockTaken);
                if (!lockTaken) throw new InvalidOperationException("Cannot taken lock.");

                if (extendedExecutionSession != null)
                {
                    extendedExecutionSession.Dispose();
                    extendedExecutionSession = null;
                }
                UpdateTasks();
            }
            finally
            {
                syncRoot.Exit();
            }
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
