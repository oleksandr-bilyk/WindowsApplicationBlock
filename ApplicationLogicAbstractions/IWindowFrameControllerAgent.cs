using System;
using System.Threading.Tasks;

namespace Tampleworks.WindowsApplicationBlock.ApplicationLogicAbstractions
{
    /// <summary>
    /// Allows to manage window content and execute tasks in its thread  dispatcher.
    /// </summary>
    public interface IWindowFrameControllerAgent
    {
        Task ShowMessageBoxOkAsync(string content);
        Task RunInViewDispatcherAsync(Action action);
        Task<StringContentDialogResult> ShowStringContentDialog(ContentDialogParameters parameters);
        IWindowFrameNavigationAgent Navigation { get; }

        /// <summary>
        /// View is unloaded in background by memory preasure.
        /// </summary>
        /// <remarks>
        /// ViewModel should unsubscribe from DataModel to avoid memory leak.
        /// </remarks>
        event Action ViewDisposing;
    }
}
