namespace Core;

public class ReadingCalendar
{
    private DateTimeProvider DateTimeProvider;

    public List<ReadingEvent> Events { get; }

    public ReadingCalendar(DateTimeProvider provider)
    {
        DateTimeProvider = provider;
        Events = new List<ReadingEvent>();
    }

    public ReadingCalendar() : this(new DateTimeProvider())
    {
    }

    public void Add(BookPlan plan)
    {
        if (plan.PagesRead == plan.PageCount)
            return;

        int pages = 1;

        var metadata = new ReadingEventMetadata(plan.Title, plan.Author);

        foreach (var session in plan.ReadingSessions)
        {
            var date = DateTime.Parse($"{session.Date} {plan.timeOfDay}");

            if (date < DateTimeProvider.Now)
                continue;

            if (session.Goal < 0)
                throw new ArgumentException("Page goal cannot be negative", nameof(session.Goal));

            Events.Add(new ReadingEvent(date, pages, session.Goal, metadata));

            if (session.Actual != 0)
            {
                pages += session.Actual;
            }
            else
            {
                pages += session.Goal;
            }
        }

        Events.Sort((a, b) => a.Date.CompareTo(b.Date));
    }
}
