using System;

namespace Tampleworks.WindowsApplicationBlock.ViewModel
{
    /// <summary>
    /// Constructs page view model and provides its unique id used for XAML data template selection.
    /// </summary>
    public interface IPageViewModelFactory
    {
        Guid PageTypeId { get; }
        object GetPageViewModel();
    }
}
