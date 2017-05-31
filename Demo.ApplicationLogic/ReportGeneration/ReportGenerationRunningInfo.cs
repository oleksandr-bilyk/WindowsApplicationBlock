namespace Tampleworks.WindowsApplicationBlock.Demo.ApplicationLogic.ReportGeneration
{
    public sealed class ReportGenerationRunningInfo
    {
        public ReportGenerationRunningInfo(
            double? progressValue,
            bool runningInExtendedExecution
        )
        {
            ProgressValue = progressValue;
            RunningInExtendedExecution = runningInExtendedExecution;
        }
        public double? ProgressValue { get; }
        public bool RunningInExtendedExecution { get; }
    }
}
