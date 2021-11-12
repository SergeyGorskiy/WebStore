using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using PhoneNumbers;
using Store.Messages;

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

        public async Task<(bool hasValue, OrderModel model)> TryGetModelAsync()
        {
            var (hasValue, order) = await TryGetOrderAsync();

            if (hasValue)
            {
                return (true, Map(order));
            }

            return (false, null);
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

        internal async Task<(bool hasValue, Order order)> TryGetOrderAsync()
        {
            if (Session.TryGetCart(out Cart cart))
            {
                var order = await _orderRepository.GetByIdAsync(cart.OrderId);
                return (true, order);
            }

            return (false, null);
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

        public OrderModel AddBook(int bookId, int count)
        {
            if (count < 1)
                throw new InvalidOperationException("Too few books to add");

            if (!TryGetOrder(out Order order))
                order = _orderRepository.Create();

            AddOrUpdateBook(order, bookId, count);
            UpdateSession(order);
            
            return Map(order);
        }

        public async Task<OrderModel> AddBookAsync(int bookId, int count)
        {
            if (count < 1)
                throw new InvalidOperationException("Too few books to add");

            var (hasValue, order) = await TryGetOrderAsync();

            if (!hasValue)
                order = await _orderRepository.CreateAsync();

            await AddOrUpdateBookAsync(order, bookId, count);
            UpdateSession(order);

            return Map(order);
        }

        private void UpdateSession(Order order)
        {
            var cart = new Cart(order.Id, order.TotalCount, order.TotalPrice);
            Session.Set(cart);
        }

        internal void AddOrUpdateBook(Order order, in int bookId, in int count)
        {
            var book = _bookRepository.GetById(bookId);

            if (order.Items.TryGet(bookId, out OrderItem orderItem))
                orderItem.Count += count;
            else
                order.Items.Add(book.Id, book.Price, count);

            _orderRepository.Update(order);
        }

        internal async Task AddOrUpdateBookAsync(Order order, int bookId, int count)
        {
            var book = await _bookRepository.GetByIdAsync(bookId);

            if (order.Items.TryGet(bookId, out OrderItem orderItem))
                orderItem.Count += count;
            else
                order.Items.Add(book.Id, book.Price, count);

            await _orderRepository.UpdateAsync(order);
        }

        public OrderModel UpdateBook(int bookId, int count)
        {
            var order = GetOrder();
            order.Items.Get(bookId).Count = count;

            _orderRepository.Update(order);
            UpdateSession(order);

            return Map(order);
        }

        public async Task<OrderModel> UpdateBookAsync(int bookId, int count)
        {
            var order = await GetOrderAsync();
            order.Items.Get(bookId).Count = count;

            await _orderRepository.UpdateAsync(order);
            UpdateSession(order);

            return Map(order);
        }

        public OrderModel RemoveBook(int bookId)
        {
            var order = GetOrder();
            order.Items.Remove(bookId);

            _orderRepository.Update(order);
            UpdateSession(order);

            return Map(order);
        }

        public async Task<OrderModel> RemoveBookAsync(int bookId)
        {
            var order = await GetOrderAsync();
            order.Items.Remove(bookId);

            await _orderRepository.UpdateAsync(order);
            UpdateSession(order);

            return Map(order);
        }

        public Order GetOrder()
        {
            if (TryGetOrder(out Order order))
                return order;
            throw new InvalidOperationException("Empty session.");
        }

        public async Task<Order> GetOrderAsync()
        {
            var (hasValue, order) = await TryGetOrderAsync();

            if (hasValue)
                return order;

            throw new InvalidOperationException("Empty session.");
        }

        public OrderModel SendConfirmation(string cellPhone)
        {
            var order = GetOrder();
            var model = Map(order);
            if (TryFormatPhone(cellPhone, out string formattedPhone))
            {
                var confirmationCode = 1111; // todo: random.Next(1000, 9999)
                model.CellPhone = formattedPhone;
                Session.SetInt32(formattedPhone, confirmationCode);
                _notificationService.SendConfirmationCode(formattedPhone, confirmationCode);
            }
            else
                model.Errors["cellPhone"] = "Номер телефона не соответствует формату +79876543210";
            
            return model;
        }
        private readonly PhoneNumberUtil _phoneNumberUtil = PhoneNumberUtil.GetInstance();
        private bool TryFormatPhone(string cellPhone, out string formattedPhone)
        {
            try
            {
                var phoneNumber = _phoneNumberUtil.Parse(cellPhone, "ru");
                formattedPhone = _phoneNumberUtil.Format(phoneNumber, PhoneNumberFormat.INTERNATIONAL);
                return true;
            }
            catch (NumberParseException)
            {
                formattedPhone = null;
                return false;
            }
        }

        public OrderModel ConfirmCellPhone(string cellPhone, int confirmationCode)
        {
            int? storedCode = Session.GetInt32(cellPhone);
            var model = new OrderModel();
            if (storedCode == null)
            {
                model.Errors["cellPhone"] = "Что-то случилось. Попробуйте получить код еще раз";
                return model;
            }

            if (storedCode != confirmationCode)
            {
                model.Errors["confirmationCode"] = "Неверный код. Проверьте и попробуйте еще раз.";
                return model;
            }

            var order = GetOrder();
            order.CellPhone = cellPhone;
            _orderRepository.Update(order);

            Session.Remove(cellPhone);

            return Map(order);
        }

        public OrderModel SetDelivery(OrderDelivery delivery)
        {
            var order = GetOrder();
            order.Delivery = delivery;
            _orderRepository.Update(order);

            return Map(order);
        }

        public OrderModel SetPayment(OrderPayment payment)
        {
            var order = GetOrder();
            order.Payment = payment;
            _orderRepository.Update(order);
            Session.RemoveCart();

            _notificationService.StartProcess(order);

            return Map(order);
        }


        
    }
}