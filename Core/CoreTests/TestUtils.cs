public class TestUtils
{
    public static Database CreateDatabase() {
        Database database = new Database("Data Source=:memory:");

        database.Open();
        database.Create(DatabaseSchema.Schema);

        return database;
    }
}
