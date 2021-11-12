using System.Collections.Generic;
using System.Threading.Tasks;

namespace Store
{
    public interface IBookRepository
    {
        Task<Book[]> GetAllByIsbnAsync(string isbn);
        Task<Book[]> GetAllByTitleOrAuthorAsync(string titleOrAuthor);

        Book GetById(int id);
        Task<Book> GetByIdAsync(int id);
        Book[] GetAllByIds(IEnumerable<int> bookIds);
    }
}