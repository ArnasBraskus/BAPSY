using Ical.Net;
using System.Text;

public class CalendarWriterTests
{
    [Fact]
    public void Test_EmptyReadingCalendar_Serialize_SerializedCalendarIsEmpty()
    {
        var TITLE = "TITLE";
        var AUTHOR = "AUTHOR";
        var EVENTS = new List<ReadingEvent> {};

        var readingCalendar = new ReadingCalendar(TITLE, AUTHOR, EVENTS);

        var bytes = CalendarWriter.Serialize(readingCalendar);

        var calendar = Calendar.Load(Encoding.ASCII.GetString(bytes));

        Assert.Empty(calendar.Events);
    }

    [Fact]
    public void Test_ReadingCalendarWithEvents_Serialize_SerializesCalendar()
    {
        var TITLE = "TITLE";
        var AUTHOR = "AUTHOR";
        var EVENTS = new List<ReadingEvent> {
            new ReadingEvent(new DateTime(2024, 04, 01), 1, 25),
            new ReadingEvent(new DateTime(2024, 04, 02), 26, 25),
            new ReadingEvent(new DateTime(2024, 04, 04), 51, 25),
            new ReadingEvent(new DateTime(2024, 04, 05), 76, 20),
        };

        var bytes = CalendarWriter.Serialize(new ReadingCalendar(TITLE, AUTHOR, EVENTS));

        var calendar = Calendar.Load(Encoding.ASCII.GetString(bytes));
        var events = calendar.Events;

        Assert.Equal(4, events.Count());

        for (int i = 0; i < EVENTS.Count(); i++)
        {
            Assert.True(EVENTS[i].Date == events[i].DtStart.Date);
        }
    }
}
