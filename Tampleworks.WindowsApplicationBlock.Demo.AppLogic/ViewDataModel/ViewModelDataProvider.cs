using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tampleworks.WindowsApplicationBlock.ViewModel;

namespace Tampleworks.WindowsApplicationBlock.Demo.AppLogic.ViewDataModel
{
    public sealed class ViewModelDataProvider : IViewModelDataProvider
    {
        private readonly MemoryController memoryController;
        private readonly IExtendedExecutionManager extendedExecutionManage;

        public ViewModelDataProvider(
            MemoryController memoryController,
            IExtendedExecutionManager extendedExecutionManage
        )
        {
            this.memoryController = memoryController;
            this.extendedExecutionManage = extendedExecutionManage;
        }

        public async Task<ReportGenerationResult> GenerateReportAsync()
        {
            using (CancellationTokenSource cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource())
            {
                var session = await extendedExecutionManage.RequestExtensionAsync(
                    "Rendering", () => cancellationTokenSource.Cancel());
                if (session != null)
                {
                    using (session)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(60), cancellationTokenSource.Token);
                        return new ReportGenerationResult();
                    }
                }
                else return null;
            }

        }

        private List<OrgData> GetAllData() => new List<OrgData>
        {
            new OrgData
            {
                Id = new Guid("3b5eb6db-5c78-4c30-99e5-b55b02d8da06"),
                Title = "Organisation 1",
                Notes = "Small organisaiton",
                Address = "Address 1",
                Logo = null,
            },
            new OrgData
            {
                Id = new Guid("3b5eb6db-5c78-4c30-99e5-b55b02d8da06"),
                Title = "Organisation 2",
                Notes = "Big organisaiton"
            },
            new OrgData
            {
                Id = new Guid("3b5eb6db-5c78-4c30-99e5-b55b02d8da06"),
                Title = "Organisation 4",
                Notes = "Big organisaiton"
            },
            new OrgData
            {
                Id = new Guid("3b5eb6db-5c78-4c30-99e5-b55b02d8da06"),
                Title = "Organisation 5",
                Notes = "Big organisaiton"
            },
        };

        public Task<OrganisationDetails> GetOrganisationDetailsAsync(Guid organisationId)
        {
            var result = (from item in GetAllData() where item.Id == organisationId select new OrganisationDetails(item.Address, item.Logo)).Single();
            return Task.FromResult(result);
        }

        public Task<List<OrganisationTitle>> GetOrganisationTitleListAsync()
        {
            var resultList = from item in GetAllData() select new OrganisationTitle(item.Id, item.Title, item.Notes);
            return Task.FromResult(resultList.ToList());
        }

        private sealed class OrgData
        {
            public Guid Id { get; set; }
            public string Title { get; set; }
            public string Notes { get; set; }
            public string Address { get; set; }
            public byte[] Logo { get; set; }
        }

        /// <summary>
        /// Activates/Launches data model on early application stage.
        /// </summary>
        public void Activate()
        {

        }

        public TimeSpan DoSomeWorkThatAllocatesMemory()
        {
            return memoryController.DoSomeWorkThatAllocatesMemory();
        }
    }
}
