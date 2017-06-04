using System;
using System.Threading.Tasks;

namespace Tampleworks.WindowsApplicationBlock.ApplicationLogicAbstractions
{
    public interface IApplicationLogicAgent
    {
        string Arguments { get; }
        IExtendedExecutionSessionFactory ExtendedExecutionSessionFactory { get; }
        IApplicationLifecycleAgent Lifecycle { get; }

        /// <summary>
        /// Sets all opened windows containt to null.
        /// </summary>
        Task ResetViewAsync();
        Task<bool> OpenNewSecondaryViewAsync(IWindowFrameControllerFactory windowFrameControllerFactory);
    }
}
