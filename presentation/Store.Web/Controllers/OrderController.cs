using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Store.Contractors;
using Store.Web.App;
using Store.Web.Contractors;
using Store.Web.Models;

namespace Store.Web.Controllers
{
    public class OrderController : Controller
    {
        private readonly OrderService _orderService;
        private readonly IEnumerable<IDeliveryService> _deliveryServices;
        private readonly IEnumerable<IPaymentService> _paymentServices;
        private readonly IEnumerable<IWebContractorService> _webContractorServices;

        public OrderController(OrderService orderService,
                               IEnumerable<IDeliveryService> deliveryServices, 
                               IEnumerable<IPaymentService> paymentServices,
                               IEnumerable<IWebContractorService> webContractorServices)
        {
            _orderService = orderService;
            _deliveryServices = deliveryServices;
            _paymentServices = paymentServices;
            _webContractorServices = webContractorServices;
            
        }

        [HttpGet]
        public IActionResult Index()
        {
            if (_orderService.TryGetModel(out OrderModel model))
                return View(model);

            return View("Empty");
        }

        [HttpPost]
        public IActionResult AddItem(int bookId, int count)
        {
            _orderService.AddBook(bookId, count);

            return RedirectToAction("Index", "Book", new { id = bookId });
        }

        [HttpPost]
        public IActionResult UpdateItem(int bookId, int count)
        {
            var model = _orderService.UpdateBook(bookId, count);
            return View("Index", model);
        }

        [HttpPost]
        public IActionResult RemoveItem(int bookId)
        {
            var model = _orderService.RemoveBook(bookId);
            return View("Index", model);
        }

        [HttpPost]
        public IActionResult SendConfirmation(string cellPhone)
        {
            var model = _orderService.SendConfirmation(cellPhone);
            return View("Confirmation", model);
        }






        //private (Order order, Cart cart) GetOrCreateOrderAndCart()
        //{
        //    Order order;
        //    if (HttpContext.Session.TryGetCart(out Cart cart))
        //    {
        //        order = _orderRepository.GetById(cart.OrderId);
        //    }
        //    else
        //    {
        //        order = _orderRepository.Create();
        //        cart = new Cart(order.Id, 0, 0m);
        //    }

        //    return (order, cart);
        //}
        //private void SaveOrderAndCart(Order order, Cart cart)
        //{
        //    _orderRepository.Update(order);

        //    cart = new Cart(order.Id, order.TotalCount, order.TotalPrice);

        //    HttpContext.Session.Set(cart);
        //}

        


        //[HttpPost]
        //public IActionResult StartDelivery(int id, string uniqueCode)
        //{
        //    var deliveryService = _deliveryServices.Single(service => service.UniqueCode == uniqueCode);
        //    var order = _orderRepository.GetById(id);
        //    var form = deliveryService.CreateForm(order);
        //    return View("DeliveryStep", form);
        //}

        //[HttpPost]
        //public IActionResult NextDelivery(int id, string uniqueCode, int step, 
        //                                  Dictionary<string, string> values)
        //{
        //    var deliveryService = _deliveryServices.Single(service => service.UniqueCode == uniqueCode);
        //    var form = deliveryService.MoveNextForm(id, step, values);
        //    if (form.IsFinal)
        //    {
        //        var order = _orderRepository.GetById(id);
        //        order.Delivery = deliveryService.GetDelivery(form);
        //        _orderRepository.Update(order);

        //        var model = new DeliveryModel
        //        {
        //            OrderId = id,
        //            Methods = _paymentServices.ToDictionary(service => service.UniqueCode,
        //                                                    service => service.Title)
        //        };

        //        return View("PaymentMethod", model);
        //    }

        //    return View("DeliveryStep", form);
        //}

        //[HttpPost]
        //public IActionResult StartPayment(int id, string uniqueCode)
        //{
        //    var paymentService = _paymentServices.Single(service => service.UniqueCode == uniqueCode);
        //    var order = _orderRepository.GetById(id);
        //    var form = paymentService.CreateForm(order);

        //    var webContractorService =
        //        _webContractorServices.SingleOrDefault(service => service.UniqueCode == uniqueCode);

        //    if (webContractorService != null)
        //        return Redirect(webContractorService.GetUri);
            
        //    return View("PaymentStep", form);
        //}

        //[HttpPost]
        //public IActionResult NextPayment(int id, string uniqueCode, int step,
        //    Dictionary<string, string> values)
        //{
        //    var paymentService = _paymentServices.Single(service => service.UniqueCode == uniqueCode);
        //    var form = paymentService.MoveNextForm(id, step, values);
        //    if (form.IsFinal)
        //    {
        //        var order = _orderRepository.GetById(id);
        //        order.Payment = paymentService.GetPayment(form);
        //        _orderRepository.Update(order);

        //        return View("Finish");
        //    }
        //    return View("PaymentStep", form);
        //}

        //public IActionResult Finish()
        //{
        //    HttpContext.Session.RemoveCart();
        //    return View();
        //}
    }
}
