using System;
using System.Threading.Tasks;
using Tampleworks.WindowsApplicationBlock.ApplicationLogicAbstractions;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;

namespace Tampleworks.WindowsApplicationBlock.ApplicationLogicEnvironment
{
    internal sealed class WindowFrameControllerAgent : IWindowFrameControllerAgent
    {
        private readonly Lazy<IWindowFrameNavigationAgent> navigation;

        public WindowFrameControllerAgent(
            CoreDispatcher dispatcher,
            Func<IWindowFrameNavigationAgent> getNavigation
        )
        {
            this.Dispatcher = dispatcher;
            navigation = new Lazy<IWindowFrameNavigationAgent>(getNavigation);
        }

        internal readonly CoreDispatcher Dispatcher;

        public IWindowFrameNavigationAgent Navigation => navigation.Value;

        public event Action ViewDisposing;

        internal void OnViewDisposing() => ViewDisposing?.Invoke();

        public async Task RunInViewDispatcherAsync(Action action) =>
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => action());

        public async Task ShowMessageBoxOkAsync(string content)
        {
            var dialog = new MessageDialog(content);
            var command = await dialog.ShowAsync();
        }

        public async Task<StringContentDialogResult> ShowStringContentDialog(ContentDialogParameters parameters)
        {
            var dialog = new ContentDialog
            {
                Title = parameters.Title,
                Content = parameters.Content,
                
            };
            if (parameters.PrimaryButtonText != null)
            {
                dialog.PrimaryButtonText = parameters.PrimaryButtonText;
            }
            if (parameters.SecondaryButtonText != null)
            {
                dialog.SecondaryButtonText = parameters.SecondaryButtonText;
            }
            var result = await dialog.ShowAsync();
            switch (result)
            {
                case ContentDialogResult.None: return StringContentDialogResult.None;
                case ContentDialogResult.Primary: return StringContentDialogResult.Primary;
                case ContentDialogResult.Secondary: return StringContentDialogResult.Secondary;
                default: throw new NotSupportedException();
            }
        }
    }
}
