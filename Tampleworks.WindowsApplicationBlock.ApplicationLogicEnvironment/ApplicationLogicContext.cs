using Tampleworks.WindowsApplicationBlock.ApplicationLogicAbstractions;

namespace Tampleworks.WindowsApplicationBlock.ApplicationLogicEnvironment
{
    internal sealed class ApplicationLogicContext
    {
        public ApplicationLogicContext(
            ApplicationLogicAgent applicationLogicAgent,
            IApplicationLogic applicaitonLogic
        )
        {
            ApplicationLogicAgent = applicationLogicAgent;
            ApplicaitonLogic = applicaitonLogic;

        }
        public ApplicationLogicAgent ApplicationLogicAgent { get; }
        public IApplicationLogic ApplicaitonLogic { get; }
    }
}
