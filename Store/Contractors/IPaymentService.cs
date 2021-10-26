using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;

namespace Store.Contractors
{
    public interface IPaymentService
    {
        public string Name { get; }
        public string Title { get; }
        Form FirstForm(Order order);
        Form NextForm(int step, IReadOnlyDictionary<string, string> values);

        OrderPayment GetPayment(Form form);
    }
}