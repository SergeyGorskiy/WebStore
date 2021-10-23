using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace Store.Web.App
{
    public class OrderService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly INotificationService _notificationService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        protected ISession Session => _httpContextAccessor.HttpContext.Session;

        public OrderService(IBookRepository bookRepository, 
                            IOrderRepository orderRepository, 
                            INotificationService notificationService, 
                            IHttpContextAccessor httpContextAccessor)
        {
            _bookRepository = bookRepository;
            _orderRepository = orderRepository;
            _notificationService = notificationService;
            _httpContextAccessor = httpContextAccessor;
        }

        public bool TryGetModel(out OrderModel model)
        {
            if (TryGetOrder(out Order order))
            {
                model = Map(order);
                return true;
            }

            model = null;
            return false;
        }

        private OrderModel Map(Order order)
        {
            var books = GetBooks(order);
            var items = from item in order.Items
                join book in books on item.BookId equals book.Id
                select new OrderItemModel
                {
                    BookId = book.Id,
                    Title = book.Title,
                    Author = book.Author,
                    Price = item.Price,
                    Count = item.Count
                };
            return new OrderModel
            {
                Id = order.Id,
                Items = items.ToArray(),
                TotalCount = order.TotalCount,
                TotalPrice = order.TotalPrice,
                CellPhone = order.CellPhone,
                DeliveryDescription = order.Delivery?.Description,
                PaymentDescription = order.Payment?.Description
            };
        }

        internal IEnumerable<Book> GetBooks(Order order)
        {
            var bookIds = order.Items.Select(item => item.BookId);
            return _bookRepository.GetAllByIds(bookIds);
        }

        internal bool TryGetOrder(out Order order)
        {
            if (Session.TryGetCart(out Cart cart))
            {
                order = _orderRepository.GetById(cart.OrderId);
                return true;
            }
            order = null;
            return false;
        }

        public OrderModel AddBook(int bookId, int count)
        {
            if (count < 1)
                throw new InvalidOperationException("Too few books to add");
            
            if (!TryGetOrder(out Order order))
            {
                AddOrUpdateBook(order, bookId, count);
                UpdateSession(order);
            }
            return Map(order);
        }

        private void UpdateSession(Order order)
        {
            var cart = new Cart(order.Id, order.TotalCount, order.TotalPrice);
            Session.Set(cart);
        }

        private void AddOrUpdateBook(Order order, in int bookId, in int count)
        {
            var book = _bookRepository.GetById(bookId);
            if (order.Items.TryGet(bookId, out OrderItem orderItem))
                orderItem.Count += count;
            else
                order.Items.Add(book.Id, book.Price, count);
        }
    }
}