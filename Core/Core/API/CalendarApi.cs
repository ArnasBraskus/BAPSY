using Ical.Net;

public class CalendarApi : ApiBase
{
    private Plans Plans;

    public CalendarApi(Users users, Plans plans) : base(users)
    {
        Plans = plans;
    }

    public IResult GetExportCalendar(HttpContext context, int id) {
        BookPlan? plan = Plans.FindPlan(id);

        if (plan is null)
            return Results.BadRequest(new ErrorResponse { Error = "Plan not found" });

        ReadingCalendar calendar = ReadingCalendar.Create(plan);

        byte[] bytes = CalendarWriter.Serialize(calendar);

        return Results.Bytes(bytes, "text/calendar");
    }

    public override void Map(WebApplication app)
    {
        app.MapGet("/calendar/{id}/export", GetExportCalendar);
    }
}
