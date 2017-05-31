namespace Tampleworks.WindowsApplicationBlock.Demo.AppLogic
{
    public sealed class AppMemoryUsageLimitChangingEventArgs
    {
        public AppMemoryUsageLimitChangingEventArgs(ulong newLimit, ulong oldLimit)
        {
            NewLimit = newLimit;
            OldLimit = oldLimit;
        }
        public ulong NewLimit { get; }
        public ulong OldLimit { get; }
    }
}
