namespace Tampleworks.WindowsApplicationBlock.Demo.AppLogic.ReportGeneration
{
    public sealed class ReportGenerationStateInfo
    {
        private ReportGenerationStateInfo(
        )
        {
        }

        public static ReportGenerationStateInfo NewRunning(ReportGenerationRunningInfo info) =>
            new ReportGenerationStateInfo
            {
                State = ReportGenerationState.Running,
                Running = info,
            };

        public static ReportGenerationStateInfo NewCancelled() =>
            new ReportGenerationStateInfo
            {
                State = ReportGenerationState.Cancelled,
            };

        public static ReportGenerationStateInfo NewFailed(ReportGenerationFaultInfo info) =>
            new ReportGenerationStateInfo
            {
                State = ReportGenerationState.Failed,
                Fault = info,
            };

        public static ReportGenerationStateInfo NewCompleted(ReportGenerationCompletionInfo info) =>
            new ReportGenerationStateInfo
            {
                State = ReportGenerationState.Completed,
                Completion = info,
            };

        public ReportGenerationState State { get; private set; }
        public ReportGenerationRunningInfo Running { get; private set; }
        public ReportGenerationFaultInfo Fault { get; private set; }
        public ReportGenerationCompletionInfo Completion { get; private set; }
    }
}