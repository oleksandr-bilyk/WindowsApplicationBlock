using System;
using System.Collections.Generic;
using System.Text;

namespace Tampleworks.WindowsApplicationBlock.ApplicationLogicAbstractions
{
    public interface IApplicationLifecycleAgent
    {
        /// <summary>
        /// It is recommended to save state from here asynchronously.
        /// </summary>
        /// <remarks>https://docs.microsoft.com/en-us/windows/uwp/launch-resume/app-lifecycle</remarks>
        event Action EnteredBackground;
        event Action LeavingBackground;
        event Action Suspension;
        event Action Resument;
    }
}
