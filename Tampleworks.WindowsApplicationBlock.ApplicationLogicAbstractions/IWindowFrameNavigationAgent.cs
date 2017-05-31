using System.Threading.Tasks;

namespace Tampleworks.WindowsApplicationBlock.ApplicationLogicAbstractions
{
    public interface IWindowFrameNavigationAgent
    {
        bool Navigate(IPageViewModelFactory viewModelFactory);
        void GoBack();
    }
}
