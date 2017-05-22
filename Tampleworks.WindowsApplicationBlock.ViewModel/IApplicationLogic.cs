namespace Tampleworks.WindowsApplicationBlock.ViewModel
{
    /// <summary>
    /// Agrigates application data and provides main window view.
    /// </summary>
    /// <remarks>
    /// </remarks>
    public interface IApplicationLogic
    {
        IWindowFrameControllerFactory MainWindowFrameControllerFactory { get; }
    }
}
