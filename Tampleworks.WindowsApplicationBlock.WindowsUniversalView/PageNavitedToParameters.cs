namespace Tampleworks.WindowsApplicationBlock.WindowsUniversalView
{
    /// <summary>
    /// Agrigates page ViewModel and View Agent.
    /// </summary>
    public sealed class PageNavitedToParameters
    {
        internal PageNavitedToParameters(object viewModel, PageViewAgent viewAgent)
        {
            ViewModel = viewModel;
            ViewAgent = viewAgent;
        }
        public object ViewModel { get; }
        public PageViewAgent ViewAgent { get; }
    }
}
