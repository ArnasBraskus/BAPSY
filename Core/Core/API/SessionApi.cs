public class SessionApi : ApiBase
{
    private Plans Plans;
    private ReadingSessions Sessions;

    public SessionApi(Users users, Plans plans, ReadingSessions sessions) : base(users)
    {
        Plans = plans;
        Sessions = sessions;
    }

    public class PostMarkSessionNoAuthRequest
    {
        public required string Token { get; set; }
        public required int SessionId { get; set; }
        public required int PagesRead { get; set; }
    }

    public class PostMarkSessionNoAuthResponse
    {

    }

    public async Task<IResult> PostMarkSessionNoAuth(HttpRequest request)
    {
        var req = await ReadJson<PostMarkSessionNoAuthRequest>(request);

        ReadingSession session = Sessions.Get(req.SessionId);

        if (req.Token != session.GenerateToken())
            return Results.BadRequest(new ErrorResponse { Error = "Bad token" });

        BookPlan plan = Plans.FindPlan(session.PlanId);

        plan.MarkReadingSession(session, req.PagesRead);

        return Results.Ok(new PostMarkSessionNoAuthResponse{});
    }

    public override void Map(WebApplication app)
    {
        app.MapPost("/sessions/mark_noauth", PostMarkSessionNoAuth);
    }
}
