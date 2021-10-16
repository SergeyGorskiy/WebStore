using System.Collections.Generic;

namespace Store.Tests
{
    public class StubBookRepository : IBookRepository
    {
        public Book[] ResultIfGetAllByIsbn { get; set; }
        public Book[] ResultOfGetAllByTitleOrAuthor { get; set; }

        public Book[] GetAllByIsbn(string isbn)
        {
            return ResultIfGetAllByIsbn;
        }

        public Book[] GetAllByTitleOrAuthor(string titleOrAuthor)
        {
            return ResultOfGetAllByTitleOrAuthor;
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