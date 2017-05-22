using System;
using System.Threading.Tasks;
using Tampleworks.WindowsApplicationBlock.ViewModel;
using Windows.UI.Xaml.Controls;

namespace Tampleworks.WindowsApplicationBlock.WindowsUniversalView
{
    internal sealed class WindowFrameNavigationAgent : IWindowFrameNavigationAgent
    {
        private readonly PageViewModelNavigator pageViewModelNavigator;
        private readonly Frame frame;

        internal WindowFrameNavigationAgent(
            PageViewModelNavigator pageViewModelNavigator, Frame frame
        )
        {
            this.pageViewModelNavigator = pageViewModelNavigator;
            this.frame = frame;
        }

        void IWindowFrameNavigationAgent.GoBack() => frame.GoBack();

        bool IWindowFrameNavigationAgent.Navigate(IPageViewModelFactory viewModelFactory)
            => pageViewModelNavigator.NavigatePageToFrame(viewModelFactory, frame);
    }
}
