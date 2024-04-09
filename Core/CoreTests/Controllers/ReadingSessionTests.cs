public class ReadingSessionTests
{
    private ReadingSessions CreateEmpty() {
        Database database = TestUtils.CreateDatabase();

        PlansTestsUtils.CreatePopulated(database);

        return new ReadingSessions(database);
    }

    private ReadingSessions CreatePopulated() {
        ReadingSessions sessions = CreateEmpty();

        sessions.Add(1, new ReadingSession("1970-01-01", 100));
        sessions.Add(1, new ReadingSession("1970-01-02", 200));

        return sessions;
    }

    [Fact]
    public void Test_EmptyDb_GetSession_Fails() {
        int ID = 99;

        ReadingSessions sessions = CreateEmpty();

        Action action = () => sessions.Get(ID);

        Assert.Throws<KeyNotFoundException>(action);
    }

    [Fact]
    public void Test_EmptyDb_AddSessionWithEmptyDate_ThrowsArgumentException() {
        int PLAN_ID = 1;
        string DATE = "";
        int GOAL = 100;

        ReadingSessions sessions = CreateEmpty();

        Action action = () => sessions.Add(PLAN_ID, new ReadingSession(DATE, GOAL));

        Assert.Throws<ArgumentException>(action);
    }

    [Fact]
    public void Test_EmptyDb_AddSessionWithNegativeGoal_ThrowsArgumentException() {
        int PLAN_ID = 1;
        string DATE = "1970-01-01";
        int GOAL = -1;

        ReadingSessions sessions = CreateEmpty();

        Action action = () => sessions.Add(PLAN_ID, new ReadingSession(DATE, GOAL));

        Assert.Throws<ArgumentException>(action);
    }

    [Fact]
    public void Test_EmptyDb_AddSessionWithValidInfoAndGetIt_InfoMatches() {
        int PLAN_ID = 1;
        string DATE = "1970-01-01";
        int GOAL = 100;

        ReadingSessions sessions = CreateEmpty();

        sessions.Add(PLAN_ID, new ReadingSession(DATE, GOAL));

        List<ReadingSession> actual = sessions.GetAll(PLAN_ID);

        Assert.Equal(1, actual.Count());
        Assert.Equal(DATE, actual[0].Date);
        Assert.Equal(GOAL, actual[0].Goal);
    }

    [Fact]
    public void Test_PopulatedDb_SetActual_ValueIsUpdated() {
        int ID = 1;
        int ACTUAL = 80;

        ReadingSessions sessions = CreatePopulated();

        sessions.SetActual(ID, ACTUAL);

        ReadingSession actual = sessions.Get(ID);

        Assert.Equal(ACTUAL, actual.Actual);
    }

    [Fact]
    public void Test_PopulatedDb_SetNegativeActual_ThrowsArgumentException() {
        int ID = 1;
        int ACTUAL = -1;

        ReadingSessions sessions = CreatePopulated();

        Action action = () => sessions.SetActual(ID, ACTUAL);

        Assert.Throws<ArgumentException>(action);
    }

    [Fact]
    public void Test_PopulatedDb_SetActualSetter_ValueIsUpdated() {
        int ID = 1;
        int ACTUAL = 80;

        ReadingSessions sessions = CreatePopulated();

        ReadingSession session = sessions.Get(ID);

        session.Actual = ACTUAL;

        ReadingSession actual = sessions.Get(ID);

        Assert.Equal(ACTUAL, actual.Actual);
    }

    [Fact]
    public void Test_PopulatedDb_SetActualSetterNoDbConnection_ThrowsException() {
        var DATE = "1970-01-01";
        var GOAL = 100;
        var ACTUAL = 80;

        ReadingSession session = new ReadingSession(DATE, GOAL);

        Action action = () => session.Actual = ACTUAL;

        Assert.Throws<InvalidOperationException>(action);
    }
}
