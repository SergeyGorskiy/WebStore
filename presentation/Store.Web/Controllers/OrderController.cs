using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using Store.Contractors;
using Store.Web.App;
using Store.Web.Contractors;

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
        public async Task<IActionResult> Index()
        {
            var (hasValue, model) = await _orderService.TryGetModelAsync();

            if (hasValue)
                return View(model);

            return View("Empty");
        }

        [HttpPost]
        public async Task<IActionResult> AddItem(int bookId, int count = 1)
        {
            await _orderService.AddBookAsync(bookId, count);

            return RedirectToAction("Index", "Book", new { id = bookId });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateItem(int bookId, int count)
        {
            var model = await _orderService.UpdateBookAsync(bookId, count);
            return View("Index", model);
        }

        [HttpPost]
        public async Task<IActionResult> RemoveItem(int bookId)
        {
            var model = await _orderService.RemoveBookAsync(bookId);

            return View("Index", model);
        }

        [HttpPost]
        public async Task<IActionResult> SendConfirmation(string cellPhone)
        {
            var model = await _orderService.SendConfirmationAsync(cellPhone);
            return View("Confirmation", model);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmCellPhone(string cellPhone, int confirmationCode)
        {
            var model = await _orderService.ConfirmCellPhoneAsync(cellPhone, confirmationCode);
            if (model.Errors.Count > 0)
                return View("Confirmation", model);

            var deliveryMethods = _deliveryServices.ToDictionary(service => service.Name, service => service.Title);

            return View("DeliveryMethod", deliveryMethods);
        }

        [HttpPost]
        public async Task<IActionResult> StartDelivery(string serviceName)
        {
            var deliveryService = _deliveryServices.Single(service => service.Name == serviceName);
            var order = await _orderService.GetOrderAsync();
            var form = deliveryService.FirstForm(order);

            var webContractorService = _webContractorServices.SingleOrDefault(service => service.Name == serviceName);
            if (webContractorService == null)
                return View("DeliveryStep", form);

            var returnUri = GetReturnUri(nameof(NextDelivery));
            var redirectUri = await webContractorService.StartSessionAsync(form.Parameters, returnUri);

            return Redirect(redirectUri.ToString());
        }

        private Uri GetReturnUri(string action)
        {
            var builder = new UriBuilder(Request.Scheme, Request.Host.Host)
            {
                Path = Url.Action(action),
                Query = null
            };
            if (Request.Host.Port != null)
                builder.Port = Request.Host.Port.Value;

            return builder.Uri;
        }

        [HttpPost]
        public async Task<IActionResult> NextDelivery(string serviceName, int step, Dictionary<string, string> values)
        {
            var deliveryService = _deliveryServices.Single(service => service.Name == serviceName);
            var form = deliveryService.NextForm(step, values);
            if (!form.IsFinal)
                return View("DeliveryStep", form);

            var delivery = deliveryService.GetDelivery(form);
            await _orderService.SetDeliveryAsync(delivery);

            var paymentMethods = _paymentServices.ToDictionary(service => service.Name, service => service.Title);

            return View("PaymentMethod", paymentMethods);
        }

        [HttpPost]
        public async Task<IActionResult> StartPayment(string serviceName)
        {
            var paymentService = _paymentServices.Single(service => service.Name == serviceName);
            var order = await _orderService.GetOrderAsync();
            var form = paymentService.FirstForm(order);

            var webContractorService = _webContractorServices.SingleOrDefault(service => service.Name == serviceName);
            if (webContractorService == null)
                return View("PaymentStep", form);

            var returnUri = GetReturnUri(nameof(NextPayment));
            var redirectUri = await webContractorService.StartSessionAsync(form.Parameters, returnUri);

            return Redirect(redirectUri.ToString());
        }

        [HttpPost]
        public async Task<IActionResult> NextPayment(string serviceName, int step, Dictionary<string, string> values)
        {
            var paymentService = _paymentServices.Single(service => service.Name == serviceName);
            var form = paymentService.NextForm(step, values);
            if (!form.IsFinal)
                return View("PaymentStep", form);

            var payment = paymentService.GetPayment(form);
            var model = await _orderService.SetPaymentAsync(payment);

            return View("Finish", model);
        }
    }
}
