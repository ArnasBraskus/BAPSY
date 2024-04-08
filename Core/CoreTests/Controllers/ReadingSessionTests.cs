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
    public void Test_PopulatedDb_SetActualSetter_ValueIsUpdated() {
        int ID = 1;
        int ACTUAL = 80;

        ReadingSessions sessions = CreatePopulated();

        ReadingSession session = sessions.Get(ID);

        Assert.NotNull(session);

        session.Actual = ACTUAL;

        ReadingSession actual = sessions.Get(ID);

        Assert.Equal(ACTUAL, actual.Actual);
    }
}
