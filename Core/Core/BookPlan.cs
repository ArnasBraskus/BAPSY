public class BookPlan
{
    public int Id { get;}
    public int UserId { get;}
    public string DeadLine { get;}
    public int DayOfWeek { get;}    //ne viena ?
    public string timeOfDay {  get;}    
    public int PagesPerDay { get;}
    public string Title { get; }
    public string Author { get; }
    public int PageCount { get; }
    public int Size { get; }

    public BookPlan(int id, int userId, string deadLine, int dayOfWeek, string timeOfDay, int pagesPerDay, 
        string title, string author, int pageCount, int size)
    {
        Id = id;
        UserId = userId;
        DeadLine = deadLine;
        DayOfWeek = dayOfWeek;
        this.timeOfDay = timeOfDay;
        PagesPerDay = pagesPerDay;
        Title = title;
        Author = author;
        PageCount = pageCount;
        Size = size;
    }
}
