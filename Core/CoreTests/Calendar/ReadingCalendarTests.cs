public class ReadingCalendarTests
{
    private BookPlan CreatePlan(string deadline, int pages, int pagesRead, List<ReadingSession> sessions)
    {
        return new BookPlan(1, 1, deadline, 3, "10:21", 0, "TEST", "AUTHOR", pages, pagesRead, sessions);
    }

    [Fact]
    public void Test_AllPagesAreRead_Create_CalendarIsEmpty()
    {
        var NOW = new DateTime(2024, 04, 10);
        var PLAN = CreatePlan("2024-04-17", 400, 400, new List<ReadingSession>());

        ReadingCalendar calendar = ReadingCalendar.Create(PLAN, NOW);

        Assert.Empty(calendar.Events);
    }

    [Fact]
    public void Test_AllSessionsArePast_Create_CalendarIsEmpty()
    {
        var NOW = new DateTime(2024, 04, 20);
        var PLAN = CreatePlan("2024-04-17", 100, 0, new List<ReadingSession> {
            new ReadingSession("2024-04-10", 25),
            new ReadingSession("2024-04-12", 25),
            new ReadingSession("2024-04-14", 25),
            new ReadingSession("2024-04-15", 25)
        });

        ReadingCalendar calendar = ReadingCalendar.Create(PLAN, NOW);

        Assert.Empty(calendar.Events);
    }

    [Fact]
    public void Test_SessionsAreValid_Create_GeneratesCalendar()
    {
        var NOW = new DateTime(2024, 04, 10);
        var PLAN = CreatePlan("2024-04-17", 100, 0, new List<ReadingSession> {
            new ReadingSession("2024-04-10", 25),
            new ReadingSession("2024-04-12", 25),
            new ReadingSession("2024-04-14", 25),
            new ReadingSession("2024-04-15", 25)
        });

        ReadingCalendar calendar = ReadingCalendar.Create(PLAN, NOW);

        Assert.Equal(PLAN.Title, calendar.BookTitle);
        Assert.Equal(PLAN.Author, calendar.BookAuthor);
        Assert.Equal(new List<ReadingEvent> {
            new ReadingEvent(new DateTime(2024, 04, 10, 10, 21, 00), 1, 25),
            new ReadingEvent(new DateTime(2024, 04, 12, 10, 21, 00), 26, 25),
            new ReadingEvent(new DateTime(2024, 04, 14, 10, 21, 00), 51, 25),
            new ReadingEvent(new DateTime(2024, 04, 15, 10, 21, 00), 76, 25)
        }, calendar.Events);
    }

    public void Test_SomeSessionsArePast_Create_GeneratesCalendar()
    {
        var NOW = new DateTime(2024, 04, 14);
        var PLAN = CreatePlan("2024-04-17", 100, 0, new List<ReadingSession> {
            new ReadingSession("2024-04-10", 25),
            new ReadingSession("2024-04-12", 25),
            new ReadingSession("2024-04-14", 25),
            new ReadingSession("2024-04-15", 25)
        });

        ReadingCalendar calendar = ReadingCalendar.Create(PLAN, NOW);

        Assert.Equal(PLAN.Title, calendar.BookTitle);
        Assert.Equal(PLAN.Author, calendar.BookAuthor);
        Assert.Equal(new List<ReadingEvent> {
            new ReadingEvent(new DateTime(2024, 04, 14, 10, 21, 00), 51, 25),
            new ReadingEvent(new DateTime(2024, 04, 15, 10, 21, 00), 76, 25)
        }, calendar.Events);

    }
}
