using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Tampleworks.WindowsApplicationBlock.ApplicationLogicAbstractions;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Tampleworks.WindowsApplicationBlock.ApplicationLogicEnvironment
{
    internal sealed class ViewManager
    {
        private readonly CoreApplicationView coreApplicationView;
        private readonly PageViewModelNavigator pageViewModelNavigator;
        private readonly IWindowFrameControllerFactory windowFrameControllerFactory;
        private readonly Lazy<IWindowFrameController> controller;
        private bool isFirstInitialization = true;
        private Frame currentFrame;

        public ViewManager(
            CoreApplicationView coreApplicationView,
            PageViewModelNavigator pageViewModelNavigator,
            IWindowFrameControllerFactory windowFrameControllerFactory,
            OpenNewViewAsyncHandler openNewViewAsync
        )
        {
            this.coreApplicationView = coreApplicationView;
            var applicationView = ApplicationView.GetForCurrentView();
            Id = applicationView.Id;
            Window = Window.Current;
            Agent = new WindowFrameControllerAgent(
                coreApplicationView.Dispatcher, openNewViewAsync, GetNavigation);
            controller = new Lazy<IWindowFrameController>(NewController);
            this.pageViewModelNavigator = pageViewModelNavigator;
            this.windowFrameControllerFactory = windowFrameControllerFactory;

            applicationView.Consolidated += OnConsolidated;
        }

        private IWindowFrameNavigationAgent GetNavigation() =>
            new WindowFrameNavigationAgent(pageViewModelNavigator, currentFrame);

        private bool NavigateCurrentPageToFrame(IPageViewModelFactory viewModelFactory)
        {
            if (currentFrame == null) throw new InvalidOperationException();
            return pageViewModelNavigator.NavigatePageToFrame(viewModelFactory, currentFrame);
        }

        private IWindowFrameController NewController() => windowFrameControllerFactory.GetWindowFrameController(Agent);

        public int Id { get; }
        public Window Window { get; }
        public WindowFrameControllerAgent Agent { get; }
        public event Action<ApplicationView, ApplicationViewConsolidatedEventArgs> Consolidated;
        private void OnConsolidated(ApplicationView sender, ApplicationViewConsolidatedEventArgs args) => Consolidated?.Invoke(sender, args);

        internal async Task InitContent()
        {
            await coreApplicationView.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal,
                () =>
                {
                    if (Window.Content != null) return;

                    currentFrame = NewFrame();

                    IPageViewModelFactory pageViewModelFactory = controller.Value.GetPageViewModelFactory();
                    pageViewModelNavigator.NavigatePageToFrame(pageViewModelFactory, currentFrame);
                    Window.Content = currentFrame;

                    if (isFirstInitialization)
                    {
                        Window.Current.Activate();
                        isFirstInitialization = false;
                    }
                }
            );
        }

        private Frame NewFrame()
        {
            var frame = new Frame();
            frame.NavigationFailed += (sender, e) => Debug.Fail("Navigation Failed.");
            frame.Navigating += Frame_Navigating;
            frame.NavigationStopped += (sender, e) => Debug.WriteLine("Navigation Stopped.");
            return frame;
        }

        private void Frame_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            
        }

        internal async Task ClearContent()
        {
            await Window.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal,
                () =>
                {
                    try
                    {
                        Agent.OnViewClosing();
                    }
                    catch
                    {
                        Debug.Fail("ViewModel detach failed.");
                        throw new InvalidOperationException();
                    }

                    Window.Content = null;
                }
            );
        }
    }
}
