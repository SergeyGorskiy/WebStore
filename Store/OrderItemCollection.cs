using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Store.Data;

namespace Store
{
    public class OrderItemCollection : IReadOnlyCollection<OrderItem>
    {
        private readonly OrderDto _orderDto;
        private readonly List<OrderItem> _items;
        public int Count => _items.Count;
        public OrderItemCollection(OrderDto orderDto)
        {
            if (orderDto == null)
                throw new ArgumentNullException(nameof(orderDto));

            _orderDto = orderDto;

            _items = orderDto.Items.Select(OrderItem.Mapper.Map).ToList();
        }

        public IEnumerator<OrderItem> GetEnumerator()
        {
            return _items.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return (_items as IEnumerable).GetEnumerator();
        }
        public OrderItem Get(int bookId)
        {
            if (TryGet(bookId, out OrderItem orderItem))
                return orderItem;
            throw new InvalidOperationException("Book not found.");
        }

        public bool TryGet(int bookId, out OrderItem orderItem)
        {
            var index = _items.FindIndex(item => item.BookId == bookId);
            if (index == -1)
            {
                orderItem = null;
                return false;
            }
            orderItem = _items[index];
            return true;
        }

        public OrderItem Add(int bookId, decimal price, int count)
        {
            if (TryGet(bookId, out OrderItem orderItem))
                throw new InvalidOperationException("Book already exists.");
            var orderItemDto = OrderItem.DtoFactory.Create(_orderDto, bookId, price, count);
            _orderDto.Items.Add(orderItemDto);

            orderItem = OrderItem.Mapper.Map(orderItemDto);
            _items.Add(orderItem);

            return orderItem;
        }

        public void Remove(int bookId)
        {
            var index = _items.FindIndex(item => item.BookId == bookId);

            if (index == -1)
                throw new InvalidOperationException("Can't find book to remove from order.");

            _orderDto.Items.RemoveAt(index);
            _items.RemoveAt(index);
        }
    }
}