public class CalendarApi : ApiBase
{
    private Plans Plans;

    public CalendarApi(Users users, Plans plans) : base(users)
    {
        Plans = plans;
    }

    public IResult GetCalendarEvents(HttpContext context, int id)
    {
        User user = GetUser(context);

        BookPlan? plan = Plans.FindPlan(id);

        if (plan is null || plan.UserId != user.Id)
            return Results.BadRequest(new ErrorResponse { Error = "Plan not found" });

        ReadingCalendar calendar = ReadingCalendar.Create(plan, DateTime.Now);

        return Results.Ok(calendar.Events);
    }

    public IResult GetExportCalendar(HttpContext context, int id)
    {
        BookPlan? plan = Plans.FindPlan(id);

        if (plan is null)
            return Results.BadRequest(new ErrorResponse { Error = "Plan not found" });

        ReadingCalendar calendar = ReadingCalendar.Create(plan, DateTime.Now);

        byte[] bytes = CalendarWriter.Serialize(calendar);

        return Results.Bytes(bytes, "text/calendar");
    }

    public override void Map(WebApplication app)
    {
        app.MapGet("/calendar/{id}/events", GetCalendarEvents).RequireAuthorization("Users");
        app.MapGet("/calendar/{id}/export", GetExportCalendar);
    }
}
