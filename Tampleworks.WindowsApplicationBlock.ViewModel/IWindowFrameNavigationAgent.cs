using System.Threading.Tasks;

namespace Tampleworks.WindowsApplicationBlock.ViewModel
{
    public interface IWindowFrameNavigationAgent
    {
        bool Navigate(IPageViewModelFactory viewModelFactory);
        void GoBack();
    }
}
