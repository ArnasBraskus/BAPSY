namespace Core;

using System.Globalization;
using System.Collections.Generic;

using static System.Runtime.InteropServices.JavaScript.JSType;

public class BookPlan
{
    private readonly Plans? Plans;
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
    public ICollection<ReadingSession> ReadingSessions { get; private set; }
    public int Finished { get; }

    public BookPlan(Plans plans, int id, int userId, string deadLine, int dayOfWeek, string timeOfDay, int pagesPerDay,
        string title, string author, int pageCount, int pagesRead, ICollection<ReadingSession> sessions, int finished)
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
        Finished = finished;
    }

    public BookPlan(int id, int userId, string deadLine, int dayOfWeek, string timeOfDay, int pagesPerDay,
        string title, string author, int pageCount, int pagesRead, ICollection<ReadingSession> sessions, int finished)
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
        Finished = finished;
    }

    public ICollection<ReadingSession> GenerateReadingSessions(DateTime startDate)
    {
        DateTime endDate = DateTime.Parse($"{DeadLine} {timeOfDay}", CultureInfo.CurrentCulture);

        var sessions = new List<ReadingSession>();

        if (startDate > endDate)
            return sessions;

        if (TimeSpan.Parse(timeOfDay, CultureInfo.CurrentCulture) < startDate.TimeOfDay)
        {
            startDate = startDate.AddDays(1);
        }

        var days = FindReadingDays(startDate, endDate);

        PagesToReadBeforeDeadline(startDate);

        int pagesLeft = PageCount - PagesRead;

        foreach (var day in days)
        {
            int goal = Math.Min(pagesLeft, PagesPerDay);

            if (goal <= 0)
                break;

            sessions.Add(new ReadingSession(day.ToString("yyyy-MM-dd", CultureInfo.CurrentCulture), goal));

            if (pagesLeft < PagesPerDay)
                break;

            pagesLeft -= PagesPerDay;
        }

        return sessions;
    }

    public ICollection<DateTime> FindReadingDays(DateTime start, DateTime end)
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
        DateTime deadline = DateTime.Parse(DeadLine, CultureInfo.CurrentCulture);
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
        if (Plans is null)
            throw new InvalidOperationException("Not connected to database");

        var realPagesRead = pagesRead - session.Actual;
        var date = DateTime.Parse(session.Date, CultureInfo.CurrentCulture).AddDays(1);

        if (realPagesRead > PageCount - PagesRead)
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
        if (Plans is null)
            throw new InvalidOperationException("Not connected to database");

		PagesRead += pagesRead;

		Plans.UpdatePagesRead(Id, PagesRead);
		ReadingSessions = Plans.UpdateReadingSessions(Id, DateTime.Now);
	}
}
