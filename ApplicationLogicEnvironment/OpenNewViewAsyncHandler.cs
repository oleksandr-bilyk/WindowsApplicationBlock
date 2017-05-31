using System.Threading.Tasks;
using Tampleworks.WindowsApplicationBlock.ApplicationLogicAbstractions;

namespace Tampleworks.WindowsApplicationBlock.ApplicationLogicEnvironment
{
    internal delegate Task<bool> OpenNewViewAsyncHandler(IWindowFrameControllerFactory windowFrameControllerFactory);
}
