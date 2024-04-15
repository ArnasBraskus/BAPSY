public class BookPlan
{
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
    public List<ReadingSession> ReadingSessions { get; }

    public BookPlan(int id, int userId, string deadLine, int dayOfWeek, string timeOfDay, int pagesPerDay,
        string title, string author, int pageCount, int pagesRead, List<ReadingSession> sessions)
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
        PagesRead = pagesRead;
        ReadingSessions = sessions;
    }

    public void PopulateReadingSessions(DateTime startDate, DateTime endDate, int pagesPerDay)
    {
        if (startDate > endDate)
            throw new ArgumentException("Start date cannot be later than end date.");

        if (PageCount <= 0)
            throw new ArgumentException("Page count must be greater than zero.", nameof(PageCount));

        var days = FindReadingDays(startDate, endDate);

        PagesToReadBeforeDeadline(startDate);

        if (PagesPerDay <= 0)
            throw new ArgumentException("Pages per day must be greater than zero.", nameof(pagesPerDay));

        foreach (var day in days)
        {
            int dayOfWeek = (int)day.DayOfWeek - 1;

            bool[] weekdays = Weekdays.FromBitField(DayOfWeek);

            if (weekdays[dayOfWeek.Equals(-1) ? 6 : dayOfWeek] && day <= endDate && PageCount >= PagesRead)
            {
                if (day == days.Last())
                {
                    ReadingSessions.Add(new ReadingSession(day.ToString("yyyy/MM/dd"), PageCount - PagesRead));
                    break;
                }
                ReadingSessions.Add(new ReadingSession(day.ToString("yyyy/MM/dd"), pagesPerDay));
            }
        }
    }

    public List<DateTime> FindReadingDays(DateTime start, DateTime end)
    {
        var days = new List<DateTime>();

        for (var day = start.Date; day.Date <= end.Date; day = day.AddDays(1))
        {
            int dayOfWeek = (int)day.DayOfWeek - 1;

            bool[] weekdays = Weekdays.FromBitField(DayOfWeek);

            if (weekdays[dayOfWeek.Equals(-1) ? 6 : dayOfWeek] && day <= end)
            {
                days.Add(day);
            }
        }
        return days;
    }

    public void PagesToReadBeforeDeadline(DateTime now)
    {
        DateTime deadline = DateTime.Parse(DeadLine);
        TimeSpan timeLeft = deadline.Subtract(now);

        if (timeLeft.Days < 0)
        {
            PagesPerDay = -1;
            return;
        }

        var daysList = FindReadingDays(now, deadline);

        int daysLeft = daysList.Count;

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