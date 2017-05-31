using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tampleworks.WindowsApplicationBlock.Demo.AppLogic.ReportGeneration;

namespace Tampleworks.WindowsApplicationBlock.Demo.AppLogic.ViewDataModel
{
    public interface IViewModelDataProvider
    {
        IReportGenerationAgent ReportGeneration { get; }
        TimeSpan DoSomeWorkThatAllocatesMemory();
        Task<List<OrganisationTitle>> GetOrganisationTitleListAsync();
        Task<OrganisationDetails> GetOrganisationDetailsAsync(Guid organisationId);
    }
}
