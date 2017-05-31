using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tampleworks.WindowsApplicationBlock.ApplicationLogicAbstractions;
using Windows.UI.Xaml.Controls;

namespace Tampleworks.WindowsApplicationBlock.ApplicationLogicEnvironment
{
    internal sealed class PageViewModelNavigator
    {
        private readonly Func<Guid, Type> getPageTypeByPageId;

        public PageViewModelNavigator(
            Func<Guid, Type> getPageTypeByPageId
        )
        {
            this.getPageTypeByPageId = getPageTypeByPageId;
        }

        public bool NavigatePageToFrame(IPageViewModelFactory pageViewModelFactory, Frame frame)
        {
            Type pageType = getPageTypeByPageId.Invoke(pageViewModelFactory.PageTypeId);
            object pageViewModel = pageViewModelFactory.GetPageViewModel();
            return frame.Navigate(pageType, new PageNavitedToParameters(pageViewModel, new PageViewAgent()));
        }
    }
}
