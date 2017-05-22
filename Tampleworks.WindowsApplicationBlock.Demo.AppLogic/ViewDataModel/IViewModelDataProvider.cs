using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tampleworks.WindowsApplicationBlock.Demo.AppLogic.ViewDataModel
{
    public interface IViewModelDataProvider
    {
        TimeSpan DoSomeWorkThatAllocatesMemory();
        Task<List<OrganisationTitle>> GetOrganisationTitleListAsync();
        Task<OrganisationDetails> GetOrganisationDetailsAsync(Guid organisationId);
        Task<ReportGenerationResult> GenerateReportAsync();
    }
}
