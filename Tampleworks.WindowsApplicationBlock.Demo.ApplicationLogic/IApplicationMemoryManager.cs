using System;

namespace Tampleworks.WindowsApplicationBlock.Demo.AppLogic
{
    public interface IApplicationMemoryManager
    {
        ulong AppMemoryUsage { get; }
        event EventHandler<AppMemoryUsageLimitChangingEventArgs> AppMemoryUsageLimitChanging;
    }
}
