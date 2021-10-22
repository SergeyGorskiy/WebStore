using System;
using Xunit;

namespace Store.Tests
{
    public class OrderItemCollectionTests
    {
        [Fact]
        public void GetItem_WithExistingItem_ReturnsItem()
        {
            var order = new Order(1, new[]
            {
                new OrderItem(1, 10m, 3),
                new OrderItem(2, 100m, 5)
            });
            var orderItem = order.Items.Get(1);
            Assert.Equal(3, orderItem.Count);
        }
        [Fact]
        public void GetItem_WithNonExistingItem_ThrowsException()
        {
            var order = new Order(1, new[]
            {
                new OrderItem(1, 10m,3),
                new OrderItem(2, 100m, 5)
            });
            Assert.Throws<InvalidOperationException>(() => order.Items.Get(100));
        }

        [Fact]
        public void AddOrUpdateItem_WithExistingItem_UpdateCount()
        {
            var order = new Order(1, new[]
            {
                new OrderItem(1, 10m, 3),
                new OrderItem(2, 100m, 5)
            });
            var book = new Book(1, null, null, null, null, 0m);
            order.Items.Add(1, 10m, 1);
            Assert.Equal(13, order.Items.Get(1).Count);
        }

        [Fact]
        public void AddOrUpdateItem_WithNonExistingItem_AddsCount()
        {
            var order = new Order(1, new[]
            {
                new OrderItem(1, 10m, 3),
                new OrderItem(2, 100m, 5)
            });
            var book = new Book(4, null, null, null, null, 0m);

            order.Items.Add(4, 30m, 10);

            Assert.Equal(10, order.Items.Get(4).Count);
        }
        [Fact]
        public void RemoveItem_WithExistingItem_RemovesItem()
        {
            var order = new Order(1, new[]
            {
                new OrderItem(1, 10m, 3),
                new OrderItem(2, 100m, 5)
            });
            order.Items.Remove(1);
            Assert.Equal(1, order.Items.Get(4).Count);
        }
        [Fact]
        public void RemoveItem_WithNonExistingItem_ThrowsException()
        {
            var order = new Order(1, new[]
            {
                new OrderItem(1, 10m, 3),
                new OrderItem(2, 100m, 5)
            });
            Assert.Throws<InvalidOperationException>(() => order.Items.Remove(100));
        }
    }

}