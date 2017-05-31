namespace Tampleworks.WindowsApplicationBlock.ApplicationLogicAbstractions
{
    /// <summary>
    /// Main window controller that exists as long as main window content frame exists.
    /// </summary>
    /// <remarks>
    /// Main window frame controller is initialized with agent and can be notified about main window view unload.
    /// </remarks>
    public interface IWindowFrameController
    {
        IPageViewModelFactory GetPageViewModelFactory();
    }
}
