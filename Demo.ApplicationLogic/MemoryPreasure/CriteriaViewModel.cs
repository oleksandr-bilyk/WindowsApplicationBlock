using System;
using Tampleworks.WindowsApplicationBlock.Demo.AppLogic.ViewDataModel;
using Tampleworks.WindowsApplicationBlock.ApplicationLogicAbstractions;

namespace Tampleworks.WindowsApplicationBlock.Demo.AppLogic.MemoryPreasure
{
    public sealed class CriteriaViewModel : ViewModelBase
    {
        private readonly IViewModelDataProvider dataModelManager;
        private readonly IWindowFrameControllerAgent viewAgent;

        public CriteriaViewModel(IViewModelDataProvider dataModelManager, IWindowFrameControllerAgent viewAgent)
        {
            this.dataModelManager = dataModelManager;
            this.viewAgent = viewAgent;
        }

        public async void Simulate()
        {
            TimeSpan timespan = dataModelManager.DoSomeWorkThatAllocatesMemory();
            await viewAgent.ShowMessageBoxOkAsync($"You have {timespan} time to move application into background.");
        }
    }
}
