namespace Tampleworks.WindowsApplicationBlock.Demo.ApplicationLogic.ViewDataModel
{
    public sealed class OrganisationDetails
    {
        public OrganisationDetails(
            string address,
            byte[] logo
        )
        {
            Address = address;
            Logo = logo;
        }
        public string Address { get; }
        public byte[] Logo { get; }
    }
}
