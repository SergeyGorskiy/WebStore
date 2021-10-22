﻿using System;

namespace Store
{
    public class OrderItem
    {
        public int BookId { get; }
        public decimal Price { get; }

        private int _count;
        public int Count
        {
            get { return _count; }
            set
            {
                ThrowIfInvalidCount(value);
                _count = value;
            }
        }

        public OrderItem(int bookId, decimal price, int count)
        {
            ThrowIfInvalidCount(count);
            BookId = bookId;
            Price = price;
            Count = count;
        }

        private static void ThrowIfInvalidCount(int count)
        {
            if (count <= 0)
            {
                throw new ArgumentOutOfRangeException("Count must be greater than zero.");
            }
        }
    }
}
