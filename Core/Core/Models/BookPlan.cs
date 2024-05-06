using static System.Runtime.InteropServices.JavaScript.JSType;

public class BookPlan
{
    private Plans Plans;
    public int Id { get; }
    public int UserId { get; }
    public string DeadLine { get; }
    public int DayOfWeek { get; }
    public string timeOfDay { get; }
    public int PagesPerDay { get; set; }
    public string Title { get; }
    public string Author { get; }
    public int PageCount { get; }
    public int PagesRead { get; private set; }
    public List<ReadingSession> ReadingSessions { get; private set; }

    public BookPlan(Plans plans, int id, int userId, string deadLine, int dayOfWeek, string timeOfDay, int pagesPerDay,
        string title, string author, int pageCount, int pagesRead, List<ReadingSession> sessions)
    {
        Plans = plans;
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

    public List<ReadingSession> GenerateReadingSessions(DateTime startDate)
    {
        DateTime endDate = DateTime.Parse($"{DeadLine} {timeOfDay}");

        if (startDate > endDate)
            throw new ArgumentException("Start date cannot be later than end date.");

        if (TimeSpan.Parse(timeOfDay) < startDate.TimeOfDay)
        {
            startDate = startDate.AddDays(1);
        }

        var days = FindReadingDays(startDate, endDate);

        PagesToReadBeforeDeadline(startDate);

        int pagesLeft = PageCount - PagesRead;

        var sessions = new List<ReadingSession>();

        foreach (var day in days)
        {
            int goal = Math.Min(pagesLeft, PagesPerDay);

            if (goal <= 0)
                break;

            sessions.Add(new ReadingSession(day.ToString("yyyy-MM-dd"), goal));

            if (pagesLeft < PagesPerDay)
                break;

            pagesLeft -= PagesPerDay;
        }

        return sessions;
    }

    public List<DateTime> FindReadingDays(DateTime start, DateTime end)
    {
        var days = new List<DateTime>();

        for (var day = start.Date; day.Date <= end.Date; day = day.AddDays(1))
        {
            bool[] weekdays = Weekdays.FromBitField(DayOfWeek);

            if (weekdays[(int)day.DayOfWeek] && day <= end)
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
            PagesPerDay = (int)Math.Ceiling((decimal)(PageCount - PagesRead) / daysLeft);
        }
    }

    public void MarkReadingSession(ReadingSession session, int pagesRead)
    {
        var realPagesRead = pagesRead - session.Actual;
        var date = DateTime.Parse(session.Date).AddDays(1);

        if (realPagesRead > PageCount - pagesRead)
            throw new InvalidOperationException("pagesRead cannot exceed remaining page count");

        session.Actual = pagesRead;
        PagesRead += realPagesRead;

        Plans.UpdatePagesRead(Id, PagesRead);

        if (session.Actual != session.Goal)
        {
            ReadingSessions = Plans.UpdateReadingSessions(Id, date);
        }
    }

    public void AdditionalPagesRead (int pagesRead)
    {
		PagesRead += pagesRead;

		Plans.UpdatePagesRead(Id, PagesRead);
		ReadingSessions = Plans.UpdateReadingSessions(Id, DateTime.Now);
	}
}
