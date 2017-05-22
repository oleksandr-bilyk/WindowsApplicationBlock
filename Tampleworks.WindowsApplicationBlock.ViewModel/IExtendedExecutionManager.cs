using System;
using System.Threading.Tasks;

namespace Tampleworks.WindowsApplicationBlock.ViewModel
{
    public interface IExtendedExecutionManager
    {
        Task<IDisposable> RequestExtensionAsync(
            string description,
            Action revokedBySystemPolicy
        );
    }
}
