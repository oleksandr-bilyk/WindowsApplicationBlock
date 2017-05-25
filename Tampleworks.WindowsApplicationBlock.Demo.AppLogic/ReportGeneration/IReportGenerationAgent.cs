using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tampleworks.WindowsApplicationBlock.Demo.AppLogic.ReportGeneration
{
    /// <summary>
    /// Services report generation process for every organisation.
    /// </summary>
    public interface IReportGenerationAgent
    {
        Task<bool> StartGenerationAsync(Guid organisationId);
        ReportGenerationStateInfo GetStateAsync(Guid organisationId);
    }
}
