namespace Tampleworks.WindowsApplicationBlock.ViewModel
{
    public interface IWindowFrameControllerFactory
    {
        IWindowFrameController GetWindowFrameController(IWindowFrameControllerAgent windowFrameControllerAgent);
    }
}
