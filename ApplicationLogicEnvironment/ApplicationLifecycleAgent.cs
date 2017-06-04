using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tampleworks.WindowsApplicationBlock.ApplicationLogicAbstractions;

namespace Tampleworks.WindowsApplicationBlock.ApplicationLogicEnvironment
{
    internal sealed  class ApplicationLifecycleAgent : IApplicationLifecycleAgent
    {
        public event Action EnteredBackground;
        public event Action LeavingBackground;
        public event Action Suspension;
        public event Action Resument;
        public event Action AppMemoryUsageLevelUpdated;

        internal void OnEnteredBackground() => EnteredBackground?.Invoke();
        internal void OnLeavingBackground() => LeavingBackground?.Invoke();
        internal void OnSuspending() => Suspension?.Invoke();
        internal void OnResument() => Resument?.Invoke();
    }
}
