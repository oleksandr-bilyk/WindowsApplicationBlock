namespace Tampleworks.WindowsApplicationBlock.Demo.AppLogic.ReportGeneration
{
    public sealed class ReportGenerationFaultInfo
    {
        public ReportGenerationFaultInfo(string faultMessage)
        {
            FaultMessage = faultMessage;
        }

        public string FaultMessage { get; }
    }
}
