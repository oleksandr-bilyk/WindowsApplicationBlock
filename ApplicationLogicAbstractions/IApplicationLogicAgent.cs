using System;
using System.Threading.Tasks;

namespace Tampleworks.WindowsApplicationBlock.ApplicationLogicAbstractions
{
    public interface IApplicationLogicAgent
    {
        string Arguments { get; }
        IExtendedExecutionSessionFactory ExtendedExecutionSessionFactory { get; }
        /// <summary>
        /// It is recommended to save state from here asynchronously.
        /// </summary>
        /// <remarks>https://docs.microsoft.com/en-us/windows/uwp/launch-resume/app-lifecycle</remarks>
        event Action EnteredBackground;
        event Action LeavingBackground;
        /// <summary>
        /// OnSuspent from Application class.
        /// </summary>
        event Action Suspension;
        event Action Resument;
        event Action AppMemoryUsageLevelUpdated;

        Task DisposeViewAsync();
    }
}
