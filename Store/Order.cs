using System;
using System.Collections.Generic;
using System.Linq;
using Store.Data;

namespace Store
{
    public class Order
    {
        private readonly OrderDto _dto;

        public int Id => _dto.Id;

        public string CellPhone
        {
            get => _dto.CellPhone;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException(nameof(CellPhone));

                _dto.CellPhone = value;
            }
        }

        public OrderDelivery Delivery
        {
            get
            {
                if (_dto.DeliveryUniqueCode == null)
                    return null;

                return new OrderDelivery(_dto.DeliveryUniqueCode,
                    _dto.DeliveryDescription, _dto.DeliveryPrice, _dto.DeliveryParameters);
            }
            set
            {
                if (value == null)
                    throw new ArgumentException(nameof(Delivery));

                _dto.DeliveryUniqueCode = value.UniqueCode;
                _dto.DeliveryDescription = value.Description;
                _dto.DeliveryPrice = value.Price;
                _dto.DeliveryParameters = value.Parameters
                    .ToDictionary(p => p.Key, p => p.Value);
            }
        }

        public OrderPayment Payment
        {
            get
            {
                if (_dto.PaymentServiceName == null)
                    return null;
                
                return new OrderPayment(_dto.PaymentServiceName, 
                    _dto.PaymentDescription, _dto.PaymentParameters);
            }
            set
            {
                if (value == null)
                    throw new ArgumentException(nameof(Payment));

                _dto.PaymentServiceName = value.UniqueCode;
                _dto.PaymentDescription = value.Description;
                _dto.PaymentParameters = value.Parameters
                    .ToDictionary(p => p.Key, p => p.Value);
            }
        }

        public OrderItemCollection Items { get; }

        public int TotalCount => Items.Sum(item => item.Count);
        
        public decimal TotalPrice => Items.Sum(item => item.Price * item.Count) 
                                     + (Delivery?.Price ?? 0m);

        public Order(OrderDto dto)
        {
            this._dto = dto;
            Items = new OrderItemCollection(dto);
        }

        public static class DtoFactory
        {
            public static OrderDto Create() => new OrderDto();
        }

        public static class Mapper
        {
            public static Order Map(OrderDto dto) => new Order(dto);

            public static OrderDto Map(Order domain) => domain._dto;
        }
    }
}