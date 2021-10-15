using System;
using System.Linq;

namespace Store.Memory
{
    public class BookRepository : IBookRepository
    {
        private readonly Book[] books =
        {
            new Book(1, "ISBN 11111-22222", "D. Knuth", "Art of programming", "description", 10m),
            new Book(2, "ISBN 11111-33333", "M. Fowler", "Refactoring", "description", 5m),
            new Book(3, "ISBN 11111-44444", "B. Kernighan", "C programming language", "description", 15m)
        };

        public Book[] GetAllByIsbn(string isbn)
        {
            return books.Where(b => b.Isbn.Equals(isbn)).ToArray();
        }

        public Book[] GetAllByTitleOrAuthor(string titleOrAuthor)
        {
            return books.Where(b => b.Title.ToUpper().Contains(titleOrAuthor.ToUpper()) 
                                    || b.Author.ToUpper().Contains(titleOrAuthor.ToUpper())).ToArray();
        }

        public Book GetById(int id)
        {
            return books.Single(b => b.Id.Equals(id));
        }
    }
}