using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;

namespace Store.Contractors
{
    public interface IDeliveryService
    {
        public string UniqueCode { get; }
        public string Title { get; }

        Form CreateForm(Order order);
        Form MoveNext(int orderId, int step, IReadOnlyDictionary<string, string> values);


    }
}