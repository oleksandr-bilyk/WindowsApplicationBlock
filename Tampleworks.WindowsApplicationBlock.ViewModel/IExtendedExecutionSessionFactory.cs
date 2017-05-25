using System;
using System.Threading.Tasks;

namespace Tampleworks.WindowsApplicationBlock.ViewModel
{
    public interface IExtendedExecutionSessionFactory
    {
        /// <returns>
        /// Returns null if extended exectuion session cannot be loaded.
        /// </returns>
        Task<IDisposable> TryRequestAsync(
            string description,
            Action revokedBySystemPolicy
        );
    }
}
