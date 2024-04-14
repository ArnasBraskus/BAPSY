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

    public static ReadingCalendar Create(BookPlan plan)
    {
        if (plan == null)
            throw new ArgumentNullException(nameof(plan));

        
        if (plan.ReadingSessions == null)
            throw new ArgumentNullException(nameof(plan.ReadingSessions));

        if (plan.PagesRead > plan.PageCount)
        throw new ArgumentException("Pages read cannot exceed total page count", nameof(plan.PagesRead));


        var events = new List<ReadingEvent>();

        ReadingCalendar calendar = new ReadingCalendar(plan.Title, plan.Author, events);

        if (plan.PagesRead == plan.PageCount)
            return calendar;

        int pages = plan.PagesRead + 1;


        foreach (var session in plan.ReadingSessions)
        {
            var date = DateTime.Parse($"{session.Date} {plan.timeOfDay}");

            if (date < DateTime.Today)
                continue;

            if (session.Goal < 0)
                throw new ArgumentException("Page goal cannot be negative", nameof(session.Goal));

            events.Add(new ReadingEvent(date, pages, session.Goal));

            pages += session.Goal;
        }

        return calendar;
    }

    public void Update(BookPlan plan)
    {
        if (plan == null)
        throw new ArgumentNullException(nameof(plan));

        Events.Clear();

        if (plan.PagesRead == plan.PageCount)
            return;

        int pages = plan.PagesRead + 1;

        if (plan.ReadingSessions == null)
            throw new ArgumentNullException(nameof(plan.ReadingSessions));

        foreach (var session in plan.ReadingSessions)
        {
            var date = DateTime.Parse($"{session.Date} {plan.timeOfDay}");

            if (date < DateTime.Today)
                continue;

            if (session.Goal < 0)
                throw new ArgumentException("Page goal cannot be negative", nameof(session.Goal));


            AddEvent(new ReadingEvent(date, pages, session.Goal));

            pages += session.Goal;
        }
    }

    public void RemoveEvent(ReadingEvent readingEvent)
    {
        if (Events.Contains(readingEvent))
            Events.Remove(readingEvent);
    }

    public void AddEvent(ReadingEvent readingEvent)
    {
        if(readingEvent != null)
            Events.Add(readingEvent);
    }
}