using System;

namespace Tampleworks.WindowsApplicationBlock.Demo.ApplicationLogic.ViewDataModel
{
    public sealed class OrganisationTitle
    {
        public OrganisationTitle(Guid organisationId, string title, string notes)
        {
            OrganisationId = organisationId;
            Title = title;
            Notes = notes;
        }
        public Guid OrganisationId { get; }
        public string Title { get; }
        public string Notes { get; }
    }
}