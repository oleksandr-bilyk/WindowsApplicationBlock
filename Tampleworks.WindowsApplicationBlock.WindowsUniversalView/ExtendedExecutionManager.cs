using System;
using System.Threading.Tasks;
using Tampleworks.WindowsApplicationBlock.ViewModel;
using Windows.ApplicationModel.ExtendedExecution;
using Windows.Foundation;

namespace Tampleworks.WindowsApplicationBlock.WindowsUniversalView
{
    internal sealed class ExtendedExecutionManager : IExtendedExecutionManager
    {
        public ExtendedExecutionManager() { }

        public Task<IDisposable> RequestExtensionAsync(string description, Action revokedBySystemPolicy)
        {
            throw new NotImplementedException();
        }
    }
}
