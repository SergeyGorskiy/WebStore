namespace Store
{
    public class Book
    {
        public string Title { get; }
        public int Id { get; }
        public Book(int id, string title)
        {
            Title = title;
            Id = id;
        }
    }
}