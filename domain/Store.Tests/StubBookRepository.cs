﻿namespace Store.Tests
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
    }
}