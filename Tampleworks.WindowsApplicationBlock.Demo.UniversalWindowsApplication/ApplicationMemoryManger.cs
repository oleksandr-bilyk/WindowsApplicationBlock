using System;
using Tampleworks.WindowsApplicationBlock.Demo.AppLogic;
using Windows.System;

namespace Tampleworks.WindowsApplicationBlock.Demo.UwpApp
{
    internal sealed class ApplicationMemoryManger : IApplicationMemoryManager
    {
        public ApplicationMemoryManger()
        {
            MemoryManager.AppMemoryUsageLimitChanging += 
                (sender, e) => OnAppMemoryUsageLimitChanging(e.NewLimit, e.OldLimit);
        }

        public ulong AppMemoryUsage => MemoryManager.AppMemoryUsage;

        public event EventHandler<Demo.AppLogic.AppMemoryUsageLimitChangingEventArgs> AppMemoryUsageLimitChanging;

        private void OnAppMemoryUsageLimitChanging(ulong newLimit, ulong oldLimit) 
            => AppMemoryUsageLimitChanging?.Invoke(this, new Demo.AppLogic.AppMemoryUsageLimitChangingEventArgs(newLimit, oldLimit));
    }
}
