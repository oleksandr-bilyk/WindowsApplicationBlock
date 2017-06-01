using System;
using System.Threading.Tasks;
using Tampleworks.WindowsApplicationBlock.ApplicationLogicAbstractions;
using Windows.ApplicationModel.ExtendedExecution;

namespace Tampleworks.WindowsApplicationBlock.ApplicationLogicEnvironment
{
    /// <summary>
    /// Provides extended execution session for applicaiton logic.
    /// </summary>
    internal sealed class ExtendedExecutionSessionFactory : IExtendedExecutionSessionFactory
    {
        public ExtendedExecutionSessionFactory() { }

        public async Task<IDisposable> TryRequestAsync(
            string description, Action revoked
        )
        {
            var session = new ExtendedExecutionSession();
            session.Reason = ExtendedExecutionReason.Unspecified;
            session.Description = description;
            session.Revoked += (sender, e) => revoked();
            ExtendedExecutionResult result = await session.RequestExtensionAsync();
            switch (result)
            {
                case ExtendedExecutionResult.Allowed: return session;
                case ExtendedExecutionResult.Denied:
                    session.Dispose();
                    return null;
                default: throw new NotSupportedException();
            }
        }
    }
}
