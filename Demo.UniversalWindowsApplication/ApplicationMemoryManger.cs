using System;
using Tampleworks.WindowsApplicationBlock.Demo.ApplicationLogic;
using Windows.System;

namespace Tampleworks.WindowsApplicationBlock.Demo.UniversalWindowsApplication
{
    internal sealed class ApplicationMemoryManger : IApplicationMemoryManager
    {
        public ApplicationMemoryManger()
        {
            MemoryManager.AppMemoryUsageLimitChanging += 
                (sender, e) => OnAppMemoryUsageLimitChanging(e.NewLimit, e.OldLimit);
        }

        public ulong AppMemoryUsage => MemoryManager.AppMemoryUsage;

        public event EventHandler<ApplicationLogic.AppMemoryUsageLimitChangingEventArgs> AppMemoryUsageLimitChanging;

        private void OnAppMemoryUsageLimitChanging(ulong newLimit, ulong oldLimit) => 
            AppMemoryUsageLimitChanging?.Invoke(
                this, 
                new ApplicationLogic.AppMemoryUsageLimitChangingEventArgs(newLimit, oldLimit)
            );
    }
}
