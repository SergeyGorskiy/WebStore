using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Store.Contractors
{
    public class PostamateDeliveryService : IDeliveryService
    {
        private static IReadOnlyDictionary<string, string> cities = new Dictionary<string, string>
        {
            {"1", "Москва" },
            {"2", "Санкт-Петербург" },
        };
        private static IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> postamates = 
            new Dictionary<string, IReadOnlyDictionary<string, string>>
            {
                {
                    "1",
                    new Dictionary<string, string>
                    {
                        { "1", "Казанский вокзал" },
                        { "2", "Охотный ряд" },
                        { "3", "Савеловский рынок" }
                    }
                },
                {
                    "2",
                    new Dictionary<string, string>
                    {
                        {"4", "Московский вокзал" },
                        {"5", "Гостиный двор" },
                        {"6", "Петропавловская крепость" }
                    }
                }
            };
        public string Name => "Postamat";
        public string Title => "Доставка через постаматы в Москве и Санкт-Петербурге.";

        public Form FirstForm(Order order)
        {
            return Form.CreateFirst(Name).AddParameter("orderId", order.Id.ToString())
                .AddField(new SelectionField("Город", "city", "1", cities));
        }

        public Form NextForm(int step, IReadOnlyDictionary<string, string> values)
        {
            if (step == 1)
            {
                if (values["city"] == "1")
                {
                    return Form.CreateNext(Name, 2, values)
                        .AddField(new SelectionField("Постамат", "postamat", "1", postamates["1"]));
                }
                if (values["city"] == "2")
                {
                    return Form.CreateNext(Name, 2, values)
                        .AddField(new SelectionField("Постамат", "postamat", "4", postamates["2"]));
                }
                else
                {
                    throw new InvalidOperationException("Invalid postamat city");
                }
            }
            if (step == 2)
            {
                return Form.CreateLast(Name, 3, values);
            }
            else
            {
                throw new InvalidOperationException("Invalid postamat step");
            }
        }

        public OrderDelivery GetDelivery(Form form)
        {
            if (form.ServiceName != Name || !form.IsFinal)
                throw new InvalidOperationException("Invalid form.");

            var cityId = form.Parameters["city"];
            var cityName = cities[cityId];
            var postamatId = form.Parameters["postamat"];
            var postamatName = postamates[cityId][postamatId];

            var parameters = new Dictionary<string, string>
            {
                { nameof(cityId), cityId },
                { nameof(cityName), cityName },
                { nameof(postamatId), postamatId },
                { nameof(postamatName), postamatName },
            };
            var description = $"Город: {cityName}\nПостамат: {postamatName}";

            return new OrderDelivery(Name, description, 150m, parameters);
        }
    }
}