using Moq;

public class TestUtils
{
    public static Database CreateDatabase() {
        Database database = new Database("Data Source=:memory:");

        database.Open();
        database.Create(DatabaseSchema.Schema);

        return database;
    }

    public static DateTimeProvider CreateDateTimeMock(DateTime now)
    {
        var mock = new Mock<DateTimeProvider>();

        mock.SetupGet(l => l.Now).Returns(now);

        return mock.Object;
    }
}
