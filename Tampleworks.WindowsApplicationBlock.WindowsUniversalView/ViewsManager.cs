using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Tampleworks.WindowsApplicationBlock.ViewModel;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Tampleworks.WindowsApplicationBlock.WindowsUniversalView
{
    /// <summary>
    /// Manages Windows lifecycle.
    /// </summary>
    internal sealed class ViewsManager
    {
        private readonly PageViewModelNavigator pageViewModelNavigator;

        public ViewsManager(
            IWindowFrameControllerFactory mainWindowFrameControllerFactory,
            Func<Guid, Type> getPageTypeByPageId
        )
        {
            pageViewModelNavigator = new PageViewModelNavigator(getPageTypeByPageId);
            var newItem = NewViewManager(CoreApplication.MainView, mainWindowFrameControllerFactory);
            views.Add(newItem);
        }

        private List<ViewManager> views = new List<ViewManager>();

        internal void OnSuspending() => views.ForEach(item => item?.Agent.OnSuspending());

        internal void OnResument() => views.ForEach(item => item?.Agent.OnResument());

        internal void OnEnteredBackground() => views.ForEach(item => item?.Agent.OnEnteredBackground());

        internal void OnLeavingBackground() => views.ForEach(item => item?.Agent.OnLeavingBackground());

        internal async Task DisposeViewAsync()
        {
            foreach (var item in views)
            {
                await item.ClearContent();
            }
        }

        internal async Task InitAllViews()
        {
            foreach (ViewManager item in views)
            {
                await item.InitContent();
            }
        }

        private ViewManager NewViewManager(CoreApplicationView coreApplicationView, IWindowFrameControllerFactory windowFrameControllerFactory)
        {
            var windowControllerContext = new ViewManager(coreApplicationView, pageViewModelNavigator, windowFrameControllerFactory, OpenNewViewAsync);
            windowControllerContext.Consolidated += ApplicationView_Consolidated;
            return windowControllerContext;
        }

        private void ApplicationView_Consolidated(ApplicationView sender, ApplicationViewConsolidatedEventArgs args)
        {
            Debug.WriteLine($"ApplicationView_Consolidated");
            /// Important: if you set breakpoint here then UWP will close wrong window. I assume (oleksandr.bilyk@live.com) that this is strange bug.
            /// Thas's why it is better to trace this even handler.
            if (args.IsUserInitiated)
            {
                Debug.WriteLine($"Was: {views.Count}");
                int removedCount = views.RemoveAll(item => item.Id == sender.Id);
                Debug.WriteLine($"Become: {views.Count} with removed removedCount.");
            }
        }

        private async Task<bool> OpenNewViewAsync(IWindowFrameControllerFactory windowFrameControllerFactory)
        {
            CoreApplicationView coreApplicationView = CoreApplication.CreateNewView();

            ViewManager windowControllerContext = default(ViewManager);
            await coreApplicationView.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal,
                async () =>
                {
                    windowControllerContext = NewViewManager(coreApplicationView, windowFrameControllerFactory);
                    await windowControllerContext.InitContent();
                }
            );
            bool viewShown = await ApplicationViewSwitcher.TryShowAsStandaloneAsync(windowControllerContext.Id);
            if (viewShown)
            {
                views.Add(windowControllerContext);
            }
            else
            {
                windowControllerContext.Agent.OnViewClosing();
                coreApplicationView.CoreWindow.Close();
            }
            return viewShown;
        }
    }
}
