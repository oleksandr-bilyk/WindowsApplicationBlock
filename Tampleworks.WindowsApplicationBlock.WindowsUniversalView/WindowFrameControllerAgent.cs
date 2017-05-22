using System;
using System.Threading.Tasks;
using Tampleworks.WindowsApplicationBlock.ViewModel;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;

namespace Tampleworks.WindowsApplicationBlock.WindowsUniversalView
{
    internal sealed class WindowFrameControllerAgent : IWindowFrameControllerAgent
    {
        private readonly OpenNewViewAsyncHandler openNewViewAsyncHandler;
        private readonly Lazy<IWindowFrameNavigationAgent> navigation;

        public WindowFrameControllerAgent(
            CoreDispatcher dispatcher,
            OpenNewViewAsyncHandler openNewViewAsyncHandler,
            Func<IWindowFrameNavigationAgent> getNavigation
        )
        {
            this.Dispatcher = dispatcher;
            this.openNewViewAsyncHandler = openNewViewAsyncHandler;
            navigation = new Lazy<IWindowFrameNavigationAgent>(getNavigation);
        }

        internal readonly CoreDispatcher Dispatcher;

        public IWindowFrameNavigationAgent Navigation => navigation.Value;

        public event Action EnteredBackground;
        public event Action LeavingBackground;
        public event Action Suspension;
        public event Action ViewDisposing;
        public event Action Resument;

        internal void OnViewClosing() => ViewDisposing?.Invoke();
        internal void OnEnteredBackground() => EnteredBackground?.Invoke();

        internal void OnLeavingBackground() => LeavingBackground?.Invoke();
        internal void OnSuspending() => Suspension?.Invoke();
        internal void OnResument() => Resument?.Invoke();

        public Task<bool> OpenNewViewAsync(IWindowFrameControllerFactory windowFrameControllerFactory) => openNewViewAsyncHandler(windowFrameControllerFactory);

        public async Task RunInViewDispatcherAsync(Action action)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => action());
        }

        public async Task ShowMessageBoxOkAsync(string content)
        {
            var dialog = new MessageDialog(content);
            var command = await dialog.ShowAsync();
        }
    }
}
