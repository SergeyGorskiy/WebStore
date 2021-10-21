namespace Store.Web.Contractors
{
    public interface IWebContractorService
    {
        public string UniqueCode { get; }
        public string GetUri { get; }
    }
}
