﻿namespace Tampleworks.WindowsApplicationBlock.ApplicationLogicAbstractions
{
    /// <summary>
    /// Agrigates application data and provides main window view.
    /// </summary>
    /// <remarks>
    /// </remarks>
    public interface IApplicationLogic
    {
        IWindowFrameControllerFactory PrimaryWindowFrameControllerFactory { get; }
    }
}
