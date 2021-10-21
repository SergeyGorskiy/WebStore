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
 
        public string UniqueCode => "Postamat";
        public string Title => "Доставка через постаматы в Москве и Санкт-Петербурге.";
        public Form CreateForm(Order order)
        {
            if (order == null)
            {
                throw new ArgumentNullException(nameof(order));
            }
            return new Form(UniqueCode, order.Id, 1, false, new[]
            {
                new SelectionField("Город", "city", "1", cities) 
            });
        }

        public Form MoveNextForm(int orderId, int step, IReadOnlyDictionary<string, string> values)
        {
            if (step == 1)
            {
                if (values["city"] == "1")
                {
                    return new Form(UniqueCode, orderId, 2, false, new Field[]
                    {
                        new HiddenField("Город", "city", "1"), 
                        new SelectionField("Постамат", "postamat", "1", postamates["1"])
                    });
                }
                else if (values["city"] == "2")
                {
                    return new Form(UniqueCode, orderId, 2, false, new Field[]
                    {
                        new HiddenField("Город", "city", "2"),
                        new SelectionField("Постамат", "postamat", "4", postamates["2"])
                    });
                }
                else
                {
                    throw new InvalidOperationException("Invalid postamat");
                }
            }
            else if (step == 2)
            {
                return new Form(UniqueCode, orderId, 3, true, new Field[]
                {
                    new HiddenField("Город", "city", values["city"]),
                    new HiddenField("Постамат", "postamat", values["postamat"]), 
                });
            }
            else
            {
                throw new InvalidOperationException("Invalid postamat step");
            }
        }

        public OrderDelivery GetDelivery(Form form)
        {
            if (form.UniqueCode != UniqueCode || !form.IsFinal)
                throw new InvalidOperationException("Invalid form.");

            var cityId = form.Fields.Single(f => f.Name == "city").Value;
            var cityName = cities[cityId];
            var postamatId = form.Fields.Single(f => f.Name == "postamat").Value;
            var postamatName = postamates[cityId][postamatId];
            var parameters = new Dictionary<string, string>
            {
                { nameof(cityId), cityId },
                { nameof(cityName), cityName },
                { nameof(postamatId), postamatId },
                { nameof(postamatName), postamatName },
            };
            var description = $"Город: {cityName}\nПостамат: {postamatName}";

            return new OrderDelivery(UniqueCode, description, 150m, parameters);
        }
    }
}