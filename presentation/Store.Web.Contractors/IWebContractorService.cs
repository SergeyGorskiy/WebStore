using System;
using System.Collections.Generic;

namespace Store.Web.Contractors
{
    public interface IWebContractorService
    {
        public string Name { get; }

        Uri StartSession(IReadOnlyDictionary<string, string> parameter, Uri returnUri);

    }
}
