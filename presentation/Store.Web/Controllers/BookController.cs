using System.Threading.Tasks;
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
        public async Task<IActionResult> Index(int id)
        {
            var model = await _bookService.GetByIdAsync(id);
            return View(model);
        }
    }
}
