using Tampleworks.WindowsApplicationBlock.ApplicationLogicAbstractions;

namespace Tampleworks.WindowsApplicationBlock.Demo.ApplicationLogic.ApplicationLogicData
{
    public interface IApplicationLogicDataProviderFactory
    {
        IApplicationLogicDataProvider Get(IApplicationLogicAgent applicationLogicAgent, MemoryController memoryBank);
    }
}
