public class TestUtils
{
    public static Database CreateDatabase() {
        Database database = new Database();

        database.Open();
        database.Create(DatabaseSchema.Schema);

        return database;
    }
}
