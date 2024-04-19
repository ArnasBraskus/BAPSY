public class ReadingEventTests
{
    [Fact]
    public static void Test_DateDiffers_Equals_ReturnsFalse()
    {
        Assert.NotEqual(new ReadingEvent(new DateTime(2024, 04, 01), 1, 20),
                        new ReadingEvent(new DateTime(2024, 04, 02), 1, 20));

    }

    [Fact]
    public static void Test_PageStartDiffers_Equals_ReturnsFalse()
    {
        Assert.NotEqual(new ReadingEvent(new DateTime(2024, 04, 01), 1, 20),
                        new ReadingEvent(new DateTime(2024, 04, 01), 2, 20));

    }

    [Fact]
    public static void Test_PagesToReadDiffers_Equals_ReturnsFalse()
    {
        Assert.NotEqual(new ReadingEvent(new DateTime(2024, 04, 01), 1, 20),
                        new ReadingEvent(new DateTime(2024, 04, 01), 1, 25));

    }

    [Fact]
    public static void Test_AllFieldsMatch_Equals_ReturnsTrue()
    {
        Assert.Equal(new ReadingEvent(new DateTime(2024, 04, 01), 1, 20),
                     new ReadingEvent(new DateTime(2024, 04, 01), 1, 20));

    }
}
