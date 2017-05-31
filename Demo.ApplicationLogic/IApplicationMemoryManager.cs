using System;

namespace Tampleworks.WindowsApplicationBlock.Demo.ApplicationLogic
{
    public interface IApplicationMemoryManager
    {
        ulong AppMemoryUsage { get; }
        event EventHandler<AppMemoryUsageLimitChangingEventArgs> AppMemoryUsageLimitChanging;
    }
}
