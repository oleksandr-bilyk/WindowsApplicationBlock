using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tampleworks.WindowsApplicationBlock.Demo.AppLogic
{
    /// <summary>
    /// Some mock memory consumer for application logic.
    /// </summary>
    public sealed class MemoryController
    {
        internal void Unallocate(ulong v)
        {
        }

        internal event Action AskApplicationToReleaseMemoryFromView;

        private void OnAskApplicationToReleaseMemoryFromView() => AskApplicationToReleaseMemoryFromView?.Invoke();

        internal TimeSpan DoSomeWorkThatAllocatesMemory()
        {
            TimeSpan timespan = TimeSpan.FromSeconds(10);
            Task.Run(
                async () =>
                {
                    await Task.Delay(timespan);
                    /// Let's assume that application allocates too much memory and asks To release view layer.
                    OnAskApplicationToReleaseMemoryFromView();
                }
            );
            return timespan;
        }
    }
}
