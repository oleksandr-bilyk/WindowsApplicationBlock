﻿using System;
using System.Threading.Tasks;

namespace Tampleworks.WindowsApplicationBlock.ViewModel
{
    public interface IWindowFrameControllerAgent
    {
        Task ShowMessageBoxOkAsync(string content);
        Task RunInViewDispatcherAsync(Action action);
        Task<bool> OpenNewViewAsync(IWindowFrameControllerFactory windowFrameControllerFactory);
        IWindowFrameNavigationAgent Navigation { get; }

        /// <summary>
        /// It is recommended to save state from here asynchronously.
        /// </summary>
        /// <remarks>https://docs.microsoft.com/en-us/windows/uwp/launch-resume/app-lifecycle</remarks>
        event Action EnteredBackground;
        event Action LeavingBackground;
        /// <summary>
        /// OnSuspent from Application class.
        /// </summary>
        event Action Suspension;
        /// <summary>
        /// View is unloaded in background by memory preasure.
        /// </summary>
        /// <remarks>
        /// ViewModel should unsubscribe from DataModel to avoid memory leak.
        /// </remarks>
        event Action ViewDisposing;
        event Action Resument;
    }
}