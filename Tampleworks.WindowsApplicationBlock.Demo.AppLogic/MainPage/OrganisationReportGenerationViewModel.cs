using System;
using System.Threading.Tasks;
using Tampleworks.WindowsApplicationBlock.Demo.AppLogic.ReportGeneration;
using Tampleworks.WindowsApplicationBlock.ViewModel;

namespace Tampleworks.WindowsApplicationBlock.Demo.AppLogic.MainPage
{
    public sealed class OrganisationReportGenerationViewModel : ViewModelBase
    {
        private readonly Guid organisationId;
        private readonly IWindowFrameControllerAgent viewAgent;
        private readonly IReportGenerationAgent reportGenerationAgent;
        private bool isRunning;
        private bool executionErrorIsVisible;
        private string executionErrorText;
        private double progressValue;
        private bool progressIsIndeterminate;

        public OrganisationReportGenerationViewModel(
            Guid organisationId,
            IWindowFrameControllerAgent viewAgent,
            IReportGenerationAgent reportGenerationAgent
        )
        {
            this.organisationId = organisationId;
            this.viewAgent = viewAgent;
            this.reportGenerationAgent = reportGenerationAgent;
            isRunning = false;
        }

        public bool IsNotRunning => !IsRunning;
        public bool IsRunning
        {
            get { return isRunning; }
            private set
            {
                if (isRunning == value) return;
                isRunning = value;
                OnPropertyChanged(nameof(IsRunning));
                OnPropertyChanged(nameof(IsNotRunning));
            }
        }

        public double ProgressValue
        {
            get { return progressValue; }
            private set
            {
                if (progressValue == value) return;
                progressValue = value;
                OnCallerPropertyChanged();
            }
        }

        public bool ProgressIsIndeterminate
        {
            get { return progressIsIndeterminate; }
            private set
            {
                if (progressIsIndeterminate == value) return;
                progressIsIndeterminate = value;
                OnCallerPropertyChanged();
            }
        }

        public bool ExecutionErrorIsVisible
        {
            get { return executionErrorIsVisible; }
            private set
            {
                if (executionErrorIsVisible == value) return;
                executionErrorIsVisible = value;
                OnCallerPropertyChanged();
            }
        }
        public string ExecutionErrorText
        {
            get { return executionErrorText; }
            private set
            {
                if (executionErrorText == value) return;
                executionErrorText = value;
                OnCallerPropertyChanged();
            }
        }

        public async void Run()
        {
            if (await reportGenerationAgent.StartGenerationAsync(organisationId))
            {
                await ObserverReportGeneration();
            }
        }

        public async Task ObserverReportGeneration()
        {
            while (true)
            {
                ReportGenerationStateInfo state = reportGenerationAgent.GetStateAsync(organisationId);
                await viewAgent.RunInViewDispatcherAsync(() => UpdateByState(state));

                if (state != null && state.State == ReportGenerationState.Running)
                {
                    await Task.Delay(1000);
                }
                else
                {
                    break;
                };
            }
        }

        private void UpdateByState(ReportGenerationStateInfo state)
        {
            if (state == null)
            {
                IsRunning = false;
                ProgressIsIndeterminate = true;
                ProgressValue = 0;
                ExecutionErrorText = string.Empty;
                executionErrorIsVisible = false;
            }
            else
            {
                switch (state.State)
                {
                    case ReportGenerationState.Running:
                        IsRunning = true;
                        ProgressIsIndeterminate = !state.Running.ProgressValue.HasValue;
                        ProgressValue = state.Running.ProgressValue ?? 0;
                        ExecutionErrorText = string.Empty;
                        ExecutionErrorIsVisible = false;
                        break;
                    case ReportGenerationState.Cancelled:
                        IsRunning = false;
                        ProgressIsIndeterminate = true;
                        ProgressValue = 0;
                        ExecutionErrorText = "Operation Cancelled";
                        executionErrorIsVisible = true;
                        break;
                    case ReportGenerationState.Failed:
                        IsRunning = false;
                        ProgressIsIndeterminate = true;
                        ProgressValue = 0;
                        ExecutionErrorText = state.Fault.FaultMessage;
                        executionErrorIsVisible = true;
                        break;
                    case ReportGenerationState.Completed:
                        IsRunning = false;
                        ProgressIsIndeterminate = true;
                        ProgressValue = 0;
                        ExecutionErrorText = string.Empty;
                        executionErrorIsVisible = false;
                        break;
                    default: throw new NotSupportedException();
                }
            }
        }
    }
}
