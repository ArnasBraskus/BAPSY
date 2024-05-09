namespace Core;

using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using Ical.Net.Serialization;
using Ical.Net;
using System.Text;

public static class CalendarWriter
{
    private const string TimeZone = "Europe/Vilnius";

    public static byte[] Serialize(ReadingCalendar calendar)
    {
        Calendar icalendar = new Calendar();

        icalendar.AddTimeZone(new VTimeZone(TimeZone));

        foreach (var e in calendar.Events)
        {
            icalendar.Events.Add(new CalendarEvent
            {
                Summary = $"Book Reading: {e.Metadata.BookTitle} by {e.Metadata.BookAuthor}",
                Description = $"Today's goal: {e.PagesToRead} pages ({e.PageStart}-{e.PageEnd})",
                Start = new CalDateTime(e.Date, TimeZone)
            });
        }

        var serializer = new CalendarSerializer();
        var serializedCalendar = serializer.SerializeToString(icalendar);

        return Encoding.ASCII.GetBytes(serializedCalendar);
    }
}
