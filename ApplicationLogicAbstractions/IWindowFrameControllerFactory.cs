namespace Tampleworks.WindowsApplicationBlock.ApplicationLogicAbstractions
{
    public interface IWindowFrameControllerFactory
    {
        IWindowFrameController GetWindowFrameController(IWindowFrameControllerAgent windowFrameControllerAgent);
    }
}
