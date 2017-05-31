using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Tampleworks.WindowsApplicationBlock.Demo.ApplicationLogic.ReportGeneration;
using Tampleworks.WindowsApplicationBlock.ApplicationLogicAbstractions;

namespace Tampleworks.WindowsApplicationBlock.Demo.ApplicationLogic.MainPage
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
        private readonly Func<bool> getRequireExtendedExecution;
        private bool runningInExtendedExecution;

        public OrganisationReportGenerationViewModel(
            Guid organisationId,
            IWindowFrameControllerAgent viewAgent,
            IReportGenerationAgent reportGenerationAgent,
            Func<bool> getRequireExtendedExecution
        )
        {
            this.organisationId = organisationId;
            this.viewAgent = viewAgent;
            this.reportGenerationAgent = reportGenerationAgent;
            isRunning = false;
            this.getRequireExtendedExecution = getRequireExtendedExecution;
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

        public bool RunningInExtendedExecution
        {
            get { return runningInExtendedExecution; }
            private set
            {
                if (runningInExtendedExecution == value) return;
                runningInExtendedExecution = value;
                OnCallerPropertyChanged();
            }
        }

        public async void Run()
        {
            try
            {
                ReportGenerationStartResult startResult;
                do
                {
                    startResult = await reportGenerationAgent.StartGenerationAsync(organisationId, getRequireExtendedExecution());
                    switch (startResult)
                    {
                        case ReportGenerationStartResult.Started: break;
                        case ReportGenerationStartResult.PreviousOperationCompleted:
                            if (await ApproveRunReportAgain())
                            {
                                reportGenerationAgent.ResetCompleted(organisationId);
                                startResult = await reportGenerationAgent.StartGenerationAsync(organisationId, getRequireExtendedExecution());
                                Debug.Assert(startResult == ReportGenerationStartResult.Started);
                                break;
                            }
                            else return;
                        default: throw new NotSupportedException();
                    }
                } while (startResult != ReportGenerationStartResult.Started);
            }
            catch (UserMessageException exc)
            {
                await viewAgent.ShowStringContentDialog(
                    new ContentDialogParameters
                    {
                        Title = "Report generation start failed.",
                        Content = exc.Message,
                        PrimaryButtonText = "OK",
                    }
                );
                return;
            }

            await ObserverReportGeneration();
        }

        private async Task<bool> ApproveRunReportAgain()
        {
            var result = await viewAgent.ShowStringContentDialog(
                new ContentDialogParameters
                {
                    Title = "Report Generation",
                    Content = "Organisation report generation was completed before. Do you want to start its generation again?",
                    PrimaryButtonText = "Ok",
                    SecondaryButtonText = "Cancel",
                }
            );
            switch (result)
            {
                case StringContentDialogResult.Primary: return true;
                case StringContentDialogResult.Secondary: return false;
                default: throw new NotSupportedException();
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
                        RunningInExtendedExecution = state.Running.RunningInExtendedExecution;
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
