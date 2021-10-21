using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Store
{
    public class Order
    {
        public int Id { get; }

        private readonly List<OrderItem> _items;
        public IReadOnlyCollection<OrderItem> Items => _items;
        public string CellPhone { get; set; }
        public OrderDelivery Delivery { get; set; }
        public OrderPayment Payment { get; set; }
        public int TotalCount => _items.Sum(item => item.Count);
        
        public decimal TotalPrice => _items.Sum(item => item.Price * item.Count) 
                                     + (Delivery?.Amount ?? 0m);

        public Order(int id, IEnumerable<OrderItem> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }
            Id = id;
            this._items = new List<OrderItem>(items);
        }
        private void ThrowBookException(string message, int bookId)
        {
            var exception = new InvalidOperationException(message);

            exception.Data["BookId"] = bookId;

            throw exception;
        }
        public OrderItem GetItem(int bookId)
        {
            int index = _items.FindIndex(item => item.BookId == bookId);
            if (index == -1)
            {
                ThrowBookException("Book not found", bookId);
            }

            return _items[index];
        }

        public void AddOrUpdateItem(Book book, int count)
        {
            if (book == null)
            {
                throw new ArgumentNullException(nameof(book));
            }
            int index = _items.FindIndex(item => item.BookId == book.Id);
            if (index == -1)
            {
                _items.Add(new OrderItem(book.Id, count, book.Price));
            }
            else
            {
                _items[index].Count += count;
            }
        }
        public void RemoveItem(int bookId)
        {
            int index = _items.FindIndex(oi => oi.BookId == bookId);

            if (index == -1)
            {
                ThrowBookException("Order does not contain specified item.", bookId);
            }

            _items.RemoveAt(index);
        }
    }
}