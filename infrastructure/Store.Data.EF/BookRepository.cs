using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Store.Data.EF
{
    public class BookRepository : IBookRepository
    {
        private readonly DbContextFactory _dbContextFactory;

        public BookRepository(DbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task<Book[]> GetAllByIsbnAsync(string isbn)
        {
            var dbContext = _dbContextFactory.Create(typeof(BookRepository));

            if (Book.TryFormatIsbn(isbn, out string formattedIsbn))
            {
                var dtos = await dbContext.Books.Where(book => book.Isbn == formattedIsbn).ToArrayAsync();

                return dtos.Select(Book.Mapper.Map).ToArray();
            }
            return new Book[0];
        }

        public async Task<Book[]> GetAllByTitleOrAuthorAsync(string titleOrAuthor)
        {
            var dbContext = _dbContextFactory.Create(typeof(BookRepository));

            var parameter = new SqlParameter("@titleOrAuthor", titleOrAuthor);

            var dtos = await dbContext.Books.FromSqlRaw(
                "SELECT * FROM Books WHERE CONTAINS((Author, Title), @titleOrAuthor)", parameter).ToArrayAsync();

            return dtos.Select(Book.Mapper.Map).ToArray();
        }

        public Book GetById(int id)
        {
            throw new System.NotImplementedException();
        }

        public async Task<Book> GetByIdAsync(int id)
        {
            var dbContext = _dbContextFactory.Create(typeof(BookRepository));

            var dto = await dbContext.Books.SingleAsync(book => book.Id == id);

            return Book.Mapper.Map(dto);
        }

        public Book[] GetAllByIds(IEnumerable<int> bookIds)
        {
            var dbContext = _dbContextFactory.Create(typeof(BookRepository));

            return dbContext.Books.Where(book => bookIds
                .Contains(book.Id)).AsEnumerable()
                .Select(Book.Mapper.Map).ToArray();
        }
    }
}