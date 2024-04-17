public class ReadingCalendar
{
    public string BookTitle { get; }
    public string BookAuthor { get; }
    public List<ReadingEvent> Events { get; }

    public ReadingCalendar(string title, string author, List<ReadingEvent> events)
    {
        BookTitle = title;
        BookAuthor = author;
        Events = events;
    }

    public static ReadingCalendar Create(BookPlan plan, DateTime now)
    {
        var events = new List<ReadingEvent>();

        ReadingCalendar calendar = new ReadingCalendar(plan.Title, plan.Author, events);

        if (plan.PagesRead == plan.PageCount)
            return calendar;

        int pages = 1;


        foreach (var session in plan.ReadingSessions)
        {
            var date = DateTime.Parse($"{session.Date} {plan.timeOfDay}");

            if (date < now)
                continue;

            events.Add(new ReadingEvent(date, pages, session.Goal));

            if (session.Actual != 0) {
                pages += session.Actual;
            }
            else {
                pages += session.Goal;
            }
        }

        return calendar;
    }
}
