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
    /// <summary>
    /// Manages one View/Window lifecycle.
    /// </summary>
    internal sealed class ViewManager
    {
        private readonly CoreApplicationView coreApplicationView;
        private readonly PageViewModelNavigator pageViewModelNavigator;
        private readonly IWindowFrameControllerFactory windowFrameControllerFactory;
        private readonly Lazy<IWindowFrameController> controller;
        private bool isFirstInitialization = true;
        private Frame currentFrame;
        private readonly Window window;
        private readonly WindowFrameControllerAgent agent;

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
            window = Window.Current;
            agent = new WindowFrameControllerAgent(
                coreApplicationView.Dispatcher, openNewViewAsync, GetNavigation);
            controller = new Lazy<IWindowFrameController>(NewController);
            this.pageViewModelNavigator = pageViewModelNavigator;
            this.windowFrameControllerFactory = windowFrameControllerFactory;

            applicationView.Consolidated += OnConsolidated;
        }

        internal int Id { get; }

        internal event Action<ApplicationView, ApplicationViewConsolidatedEventArgs> Consolidated;

        #region Methods
        private IWindowFrameNavigationAgent GetNavigation() =>
            new WindowFrameNavigationAgent(pageViewModelNavigator, currentFrame);

        private bool NavigateCurrentPageToFrame(IPageViewModelFactory viewModelFactory)
        {
            if (currentFrame == null) throw new InvalidOperationException();
            return pageViewModelNavigator.NavigatePageToFrame(viewModelFactory, currentFrame);
        }

        private IWindowFrameController NewController() => windowFrameControllerFactory.GetWindowFrameController(agent);

        private void OnConsolidated(ApplicationView sender, ApplicationViewConsolidatedEventArgs args) => Consolidated?.Invoke(sender, args);

        internal async Task InitContent()
        {
            await coreApplicationView.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal,
                () =>
                {
                    if (window.Content != null) return;

                    currentFrame = NewFrame();

                    IPageViewModelFactory pageViewModelFactory = controller.Value.GetPageViewModelFactory();
                    pageViewModelNavigator.NavigatePageToFrame(pageViewModelFactory, currentFrame);
                    window.Content = currentFrame;

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

        internal void RiseViewClosing() => agent.OnViewClosing();
        internal void RiseSuspending() => agent.OnSuspending();
        internal void RiseResument() => agent.OnResument();
        internal void RiseEnteredBackground() => agent.OnEnteredBackground();
        internal void RiseLeavingBackground() => agent.OnLeavingBackground();

        internal async Task ClearContent()
        {
            await window.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal,
                () =>
                {
                    try
                    {
                        agent.OnViewClosing();
                    }
                    catch
                    {
                        Debug.Fail("ViewModel detach failed.");
                        throw new InvalidOperationException();
                    }

                    window.Content = null;
                }
            );
        } 
        #endregion
    }
}
