using System;
using System.Collections;
using System.Collections.Generic;

namespace Store
{
    public class OrderItemCollection : IReadOnlyCollection<OrderItem>
    {
        private readonly List<OrderItem> _items;
        public int Count => _items.Count;
        public OrderItemCollection(IEnumerable<OrderItem> items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));
            this._items = new List<OrderItem>(items);
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
                throw new InvalidOperationException("Book already found.");
            orderItem = new OrderItem(bookId, price, count);
            _items.Add(orderItem);

            return orderItem;
        }

        public void Remove(int bookId)
        {
            _items.Remove(Get(bookId));
        }
    }
}