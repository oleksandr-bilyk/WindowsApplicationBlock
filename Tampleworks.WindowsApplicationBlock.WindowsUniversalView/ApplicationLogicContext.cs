using Tampleworks.WindowsApplicationBlock.ViewModel;

namespace Tampleworks.WindowsApplicationBlock.WindowsUniversalView
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
