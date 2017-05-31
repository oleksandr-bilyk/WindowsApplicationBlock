namespace Tampleworks.WindowsApplicationBlock.Demo.ApplicationLogic.ReportGeneration
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
