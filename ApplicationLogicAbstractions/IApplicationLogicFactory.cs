namespace Tampleworks.WindowsApplicationBlock.ApplicationLogicAbstractions
{
    /// <summary>
    /// Application root logic factory.
    /// </summary>
    /// <remarks>
    /// Class may have default constructor and will create application logic.
    /// </remarks>
    public interface IApplicationLogicFactory
    {
        IApplicationLogic GetApplicationLogic(IApplicationLogicAgent agent);
    }
}
