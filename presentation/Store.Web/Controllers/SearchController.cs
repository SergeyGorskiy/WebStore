using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Store.Web.App;

namespace Store.Web.Controllers
{
    public class SearchController : Controller
    {
        private readonly BookService _bookService;

        public SearchController(BookService bookService)
        {
            _bookService = bookService;
        }

        public async Task<IActionResult> Index(string query)
        {
            var books = await _bookService.GetAllByQueryAsync(query);

            return View("Index", books);
        }
    }
}
