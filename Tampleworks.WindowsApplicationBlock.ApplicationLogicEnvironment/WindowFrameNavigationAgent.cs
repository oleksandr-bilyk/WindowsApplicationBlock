using System;
using System.Threading.Tasks;
using Tampleworks.WindowsApplicationBlock.ApplicationLogicAbstractions;
using Windows.UI.Xaml.Controls;

namespace Tampleworks.WindowsApplicationBlock.ApplicationLogicEnvironment
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
