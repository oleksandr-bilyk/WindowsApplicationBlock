using Tampleworks.WindowsApplicationBlock.Demo.ApplicationLogic.ApplicationLogicData;
using Tampleworks.WindowsApplicationBlock.Demo.ApplicationLogic.Tracing;
using Tampleworks.WindowsApplicationBlock.ApplicationLogicAbstractions;

namespace Tampleworks.WindowsApplicationBlock.Demo.ApplicationLogic
{
    public sealed class ApplicationLogicFactory : IApplicationLogicFactory
    {
        private readonly IApplicationLogicDataProviderFactory dataModelManagerFactory;
        private readonly IApplicationMemoryManager memoryManager;
        private readonly ISemanticLogger logger;

        public ApplicationLogicFactory(
            IApplicationLogicDataProviderFactory dataModelManagerFactory,
            IApplicationMemoryManager memoryManager,
            ISemanticLogger logger
        )
        {
            this.dataModelManagerFactory = dataModelManagerFactory;
            this.memoryManager = memoryManager;
            this.logger = logger;
        }

        public IApplicationLogic GetApplicationLogic(IApplicationLogicAgent applicationLogicAgent)
            => new ApplicationLogic(
                applicationLogicAgent, 
                dataModelManagerFactory,
                memoryManager,
                logger
            );
    }
}
