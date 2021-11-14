using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Store.Web.Contractors
{
    public interface IWebContractorService
    {
        public string Name { get; }

        Uri StartSession(IReadOnlyDictionary<string, string> parameter, Uri returnUri);

        Task<Uri> StartSessionAsync(IReadOnlyDictionary<string, string> formParameters, Uri returnUri);
    }
}
