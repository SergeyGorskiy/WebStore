using System.Collections.Generic;

namespace Store.Data.EF
{
    public class BookRepository : IBookRepository
    {
        public Book[] GetAllByIsbn(string isbn)
        {
            throw new System.NotImplementedException();
        }

        public Book[] GetAllByTitleOrAuthor(string titleOrAuthor)
        {
            throw new System.NotImplementedException();
        }

        public Book GetById(int id)
        {
            throw new System.NotImplementedException();
        }

        public Book[] GetAllByIds(IEnumerable<int> bookIds)
        {
            throw new System.NotImplementedException();
        }
    }
}