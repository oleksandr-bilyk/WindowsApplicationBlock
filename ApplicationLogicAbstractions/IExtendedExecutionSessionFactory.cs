using System;
using System.Threading.Tasks;

namespace Tampleworks.WindowsApplicationBlock.ApplicationLogicAbstractions
{
    /// <summary>
    /// Provides extended execution session.
    /// </summary>
    public interface IExtendedExecutionSessionFactory
    {
        /// <returns>
        /// Returns null if extended exectuion session cannot be loaded.
        /// </returns>
        /// <remarks>
        /// Only one extended execution session may be requested at one time. Before getting the second 
        /// session the first one must be disposed.
        /// </remarks>
        Task<IDisposable> TryRequestAsync(
            string description,
            Action revokedBySystemPolicy
        );
    }
}
