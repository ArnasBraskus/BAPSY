public class BookPlan
{
    private ReadingSessions Sessions;

    public int Id { get; }
    public int UserId { get; }
    public string DeadLine { get; }
    public int DayOfWeek { get; }
    public string timeOfDay { get; }
    public int PagesPerDay { get; set; }
    public string Title { get; }
    public string Author { get; }
    public int PageCount { get; }
    public int PagesRead { get; }

    public BookPlan(Database db, int id, int userId, string deadLine, int dayOfWeek, string timeOfDay, int pagesPerDay,
        string title, string author, int pageCount, int pagesRead)
    {
        Sessions = new ReadingSessions(db);
        Id = id;
        UserId = userId;
        DeadLine = deadLine;
        DayOfWeek = dayOfWeek;
        this.timeOfDay = timeOfDay;
        PagesPerDay = pagesPerDay;
        Title = title;
        Author = author;
        PageCount = pageCount;
        PagesRead = pagesRead;
    }

    public List<ReadingSession> ReadingSessions { get => Sessions.GetAll(Id); }

    public void PagesToReadBeforeDeadline(DateTime now)
    {
        DateTime deadline = DateTime.Parse(DeadLine);
        TimeSpan timeLeft = deadline.Subtract(now);
        int daysLeft = 0;

        if (timeLeft.Days < 0)
        {
            PagesPerDay = -1;
            return;
        }

        for (int i = 0; i < timeLeft.Days + 1; i++)
        {
            DateTime date = now.AddDays(i);
            int dayOfWeek = (int)date.DayOfWeek - 1;

            bool[] weekdays = Weekdays.FromBitField(DayOfWeek);

            if (weekdays[dayOfWeek.Equals(-1) ? 6 : dayOfWeek] && date <= deadline)
            {
                daysLeft++;
            }
        }
        if (daysLeft > 0)
        {
            PagesPerDay = (int)Math.Ceiling((decimal)PageCount / daysLeft);
        }
        else
        {
            PagesPerDay = -1;
        }
    }
}
