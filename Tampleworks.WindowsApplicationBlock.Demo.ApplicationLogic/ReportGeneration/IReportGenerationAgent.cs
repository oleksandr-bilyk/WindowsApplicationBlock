using System;
using System.Threading.Tasks;

namespace Tampleworks.WindowsApplicationBlock.Demo.AppLogic.ReportGeneration
{
    /// <summary>
    /// Services report generation process for every organisation.
    /// </summary>
    public interface IReportGenerationAgent
    {
        Task<ReportGenerationStartResult> StartGenerationAsync(Guid organisationId, bool requireNonbreakingBehavior);
        ReportGenerationStateInfo GetStateAsync(Guid organisationId);
        void ResetCompleted(Guid organisationId);
        void CancelByUser(Guid organisationId);
    }
}
