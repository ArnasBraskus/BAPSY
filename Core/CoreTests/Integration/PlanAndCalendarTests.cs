public class PlanAndCalendarTests
{
    private static readonly (string, bool[], string, string, string, int)[] TestPlans = new (string, bool[], string, string, string, int)[] {
        ("2024-04-15", [false, true, true, false, false, false, false], "12:28", "Title1", "Author1", 122),
        ("2024-05-20", [false, true, false, false, false, false, false], "14:38", "Title2", "Author2", 254),
        ("2024-04-01", [true, true, true, true, true, true, true], "17:30", "Title3", "Author3", 344),
        ("2024-04-22", [true, false, false, false, false, false, true], "10:00", "Title4", "Author4", 191),
        ("2024-04-30", [true, false, true, false, true, false, true], "13:45", "Title5", "Author5", 201),
    };

    public static IEnumerable<object[]> GetTestPlans()
    {
        foreach (var plan in TestPlans)
        {
            yield return new object[] { plan.Item1, plan.Item2, plan.Item3, plan.Item4, plan.Item5, plan.Item6 };
        }
    }

    [Theory]
    [MemberData(nameof(GetTestPlans), MemberType = typeof(PlanAndCalendarTests))]
    public static void Test_CreatePlan_AllPagesAreAccountedFor(string deadline, bool[] days, string time, string title, string author, int pages)
    {
        var NOW = new DateTime(2024, 04, 01);
        var USER_ID = 1;

        Database database = TestUtils.CreateDatabase();

        Users users = UserTestsUtils.CreatePopulated(database);
        Plans plans = new Plans(database);

        int id = plans.AddPlan(users.FindUser(USER_ID)!, deadline, Weekdays.ToBitField(days), time, 0, title, author, pages);

        plans.UpdateReadingSessions(id, NOW);

        BookPlan? plan = plans.FindPlan(id)!;

        ReadingCalendar calendar = ReadingCalendar.Create(plan, NOW);

        int totalPages = calendar.Events.Sum(x => x.PagesToRead);

        Assert.Equal(pages, totalPages);
    }

    [Fact]
    public static void Test_CreatePlan_ReadingEventsAreValid()
    {
        var USER_ID = 1;
        var WEEKDAYS = Weekdays.Monday | Weekdays.Tuesday | Weekdays.Friday;
        var NOW = new DateTime(2024, 04, 10);
        var DEADLINE = "2024-04-30";
        var TIME = "10:15";
        var TITLE = "Title";
        var AUTHOR = "Author";
        var PAGES = 300;

        Database database = TestUtils.CreateDatabase();

        Users users = UserTestsUtils.CreatePopulated(database);
        Plans plans = new Plans(database);

        int id = plans.AddPlan(users.FindUser(USER_ID)!, DEADLINE, WEEKDAYS, TIME, 0, TITLE, AUTHOR, PAGES);

        plans.UpdateReadingSessions(id, NOW);

        BookPlan? plan = plans.FindPlan(id)!;

        ReadingCalendar calendar = ReadingCalendar.Create(plan, NOW);

        var expected = new List<ReadingEvent> {
            new ReadingEvent(new DateTime(2024, 4, 12, 10, 15, 0), 1, 34),
            new ReadingEvent(new DateTime(2024, 4, 15, 10, 15, 0), 35, 34),
            new ReadingEvent(new DateTime(2024, 4, 16, 10, 15, 0), 69, 34),
            new ReadingEvent(new DateTime(2024, 4, 19, 10, 15, 0), 103, 34),
            new ReadingEvent(new DateTime(2024, 4, 22, 10, 15, 0), 137, 34),
            new ReadingEvent(new DateTime(2024, 4, 23, 10, 15, 0), 171, 34),
            new ReadingEvent(new DateTime(2024, 4, 26, 10, 15, 0), 205, 34),
            new ReadingEvent(new DateTime(2024, 4, 29, 10, 15, 0), 239, 34),
            new ReadingEvent(new DateTime(2024, 4, 30, 10, 15, 0), 273, 28)
        };

        Assert.Equal(expected, calendar.Events);
    }

    [Fact]
    public static void Test_CreatePlanAndMarkReadingEvent_CalendarIsUpdated()
    {
        var USER_ID = 1;
        var WEEKDAYS = Weekdays.Monday | Weekdays.Tuesday | Weekdays.Friday;
        var NOW = new DateTime(2024, 04, 10);
        var DEADLINE = "2024-04-30";
        var TIME = "10:15";
        var TITLE = "Title";
        var AUTHOR = "Author";
        var PAGES = 300;

        Database database = TestUtils.CreateDatabase();

        Users users = UserTestsUtils.CreatePopulated(database);
        Plans plans = new Plans(database);
        ReadingSessions sessions = new ReadingSessions(database);

        int id = plans.AddPlan(users.FindUser(USER_ID)!, DEADLINE, WEEKDAYS, TIME, 0, TITLE, AUTHOR, PAGES);

        plans.UpdateReadingSessions(id, NOW);

        var all = sessions.GetAll(id);
        var session = all.First();

        BookPlan? plan = plans.FindPlan(id)!;

        plan.MarkReadingSession(session, 50);

        ReadingCalendar calendar = ReadingCalendar.Create(plan, NOW);

        var expected = new List<ReadingEvent> {
            new ReadingEvent(new DateTime(2024, 4, 12, 10, 15, 0), 1, 34),
            new ReadingEvent(new DateTime(2024, 4, 15, 10, 15, 0), 51, 32),
            new ReadingEvent(new DateTime(2024, 4, 16, 10, 15, 0), 83, 32),
            new ReadingEvent(new DateTime(2024, 4, 19, 10, 15, 0), 115, 32),
            new ReadingEvent(new DateTime(2024, 4, 22, 10, 15, 0), 147, 32),
            new ReadingEvent(new DateTime(2024, 4, 23, 10, 15, 0), 179, 32),
            new ReadingEvent(new DateTime(2024, 4, 26, 10, 15, 0), 211, 32),
            new ReadingEvent(new DateTime(2024, 4, 29, 10, 15, 0), 243, 32),
            new ReadingEvent(new DateTime(2024, 4, 30, 10, 15, 0), 275, 26)
        };

        Assert.Equal(expected, calendar.Events);
    }

    [Fact]
    public static void Test_CreatePlanAndCompleteEarly_CalendarIsEmpty()
    {
        var USER_ID = 1;
        var WEEKDAYS = Weekdays.Monday | Weekdays.Tuesday | Weekdays.Friday;
        var NOW = new DateTime(2024, 04, 10);
        var DEADLINE = "2024-04-30";
        var TIME = "10:15";
        var TITLE = "Title";
        var AUTHOR = "Author";
        var PAGES = 300;

        Database database = TestUtils.CreateDatabase();

        Users users = UserTestsUtils.CreatePopulated(database);
        Plans plans = new Plans(database);
        ReadingSessions sessions = new ReadingSessions(database);

        int id = plans.AddPlan(users.FindUser(USER_ID)!, DEADLINE, WEEKDAYS, TIME, 0, TITLE, AUTHOR, PAGES);

        plans.UpdateReadingSessions(id, NOW);

        var all = sessions.GetAll(id);

        BookPlan? plan = plans.FindPlan(id)!;

        plan.MarkReadingSession(all[0], 50);
        plan.MarkReadingSession(all[1], 100);
        plan.MarkReadingSession(all[2], 150);

        ReadingCalendar calendar = ReadingCalendar.Create(plan, NOW);

        Assert.Empty(calendar.Events);
    }

    [Fact]
    public static void Test_CreatePlanAndAndEdit_CalendarIsUpdated()
    {
        var USER_ID = 1;
        var WEEKDAYS = Weekdays.Monday | Weekdays.Tuesday | Weekdays.Friday;
        var WEEKDAYS_EDIT = Weekdays.Monday | Weekdays.Friday;
        var NOW = new DateTime(2024, 04, 10);
        var DEADLINE = "2024-04-30";
        var TIME = "10:15";
        var TITLE = "Title";
        var AUTHOR = "Author";
        var PAGES = 300;

        Database database = TestUtils.CreateDatabase();

        Users users = UserTestsUtils.CreatePopulated(database);
        Plans plans = new Plans(database);
        ReadingSessions sessions = new ReadingSessions(database);

        int id = plans.AddPlan(users.FindUser(USER_ID)!, DEADLINE, WEEKDAYS, TIME, 0, TITLE, AUTHOR, PAGES);

        plans.UpdateReadingSessions(id, NOW);

        plans.UpdatePlan(id, DEADLINE, WEEKDAYS_EDIT, TIME, TITLE, AUTHOR, PAGES);
        plans.UpdateReadingSessions(id, NOW);

        BookPlan? plan = plans.FindPlan(id)!;

        ReadingCalendar calendar = ReadingCalendar.Create(plan, NOW);

        var expected = new List<ReadingEvent> {
            new ReadingEvent(new DateTime(2024, 4, 12, 10, 15, 0), 1, 50),
            new ReadingEvent(new DateTime(2024, 4, 15, 10, 15, 0), 51, 50),
            new ReadingEvent(new DateTime(2024, 4, 19, 10, 15, 0), 101, 50),
            new ReadingEvent(new DateTime(2024, 4, 22, 10, 15, 0), 151, 50),
            new ReadingEvent(new DateTime(2024, 4, 26, 10, 15, 0), 201, 50),
            new ReadingEvent(new DateTime(2024, 4, 29, 10, 15, 0), 251, 50)
        };

        Assert.Equal(expected, calendar.Events);
    }

}
