
    public class Book
    {
    public int Id { get; }
    public int PlanID { get; }
    public string Title { get; }
    public string Author { get; }
    public int PageCount { get; }
    public int Size { get; }

    public Book(int id, int planid, string title, string author, int pageCount, int size)
    {
        Id = id;
        PlanID = planid; 
        Title = title;
        Author = author;
        PageCount = pageCount;
        Size = size;
    }
}
