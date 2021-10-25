using Microsoft.AspNetCore.Mvc;
using Store.Web.App;

namespace Store.Web.Controllers
{
    public class BookController : Controller
    {
        private readonly BookService _bookService;

        public BookController(BookService bookService)
        {
            this._bookService = bookService;
        }
        public IActionResult Index(int id)
        {
            var model = _bookService.GetById(id);
            return View(model);
        }
    }
}
