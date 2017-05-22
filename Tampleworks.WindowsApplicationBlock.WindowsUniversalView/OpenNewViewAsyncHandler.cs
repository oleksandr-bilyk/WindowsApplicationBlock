using System.Threading.Tasks;
using Tampleworks.WindowsApplicationBlock.ViewModel;

namespace Tampleworks.WindowsApplicationBlock.WindowsUniversalView
{
    internal delegate Task<bool> OpenNewViewAsyncHandler(IWindowFrameControllerFactory windowFrameControllerFactory);
}
