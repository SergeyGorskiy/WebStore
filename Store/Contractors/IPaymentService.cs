using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;

namespace Store.Contractors
{
    public interface IPaymentService
    {
        public string UniqueCode { get; }
        public string Title { get; }
        Form CreateForm(Order order);
        Form MoveNextForm(int orderId, int step, IReadOnlyDictionary<string, string> values);

        OrderPayment GetPayment(Form form);
    }
}