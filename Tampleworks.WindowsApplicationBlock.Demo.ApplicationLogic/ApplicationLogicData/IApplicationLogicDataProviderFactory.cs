using Tampleworks.WindowsApplicationBlock.ApplicationLogicAbstractions;

namespace Tampleworks.WindowsApplicationBlock.Demo.AppLogic.ApplicationLogicData
{
    public interface IApplicationLogicDataProviderFactory
    {
        IApplicationLogicDataProvider Get(IApplicationLogicAgent applicationLogicAgent, MemoryController memoryBank);
    }
}
