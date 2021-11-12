using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Store.Web.App
{
    public class BookService
    {
        private readonly IBookRepository _bookRepository;

        public BookService(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public async Task<BookModel> GetByIdAsync(int id)
        {
            var book = await _bookRepository.GetByIdAsync(id);

            return Map(book);
        }

        public async Task<IReadOnlyCollection<BookModel>> GetAllByQueryAsync(string query)
        {
            var books = Book.IsIsbn(query)
                ? await _bookRepository.GetAllByIsbnAsync(query)
                : await _bookRepository.GetAllByTitleOrAuthorAsync(query);

            return books.Select(Map).ToArray();
        }

        private BookModel Map(Book book)
        {
            return new BookModel
            {
                Id = book.Id,
                Isbn = book.Isbn,
                Title = book.Title,
                Author = book.Author,
                Description = book.Description,
                Price = book.Price
            };
        }
    }
}