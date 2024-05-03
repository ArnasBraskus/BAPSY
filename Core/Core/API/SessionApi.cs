public class SessionApi : ApiBase
{
    private Plans Plans;
    private ReadingSessions Sessions;

    public SessionApi(Users users, Plans plans, ReadingSessions sessions) : base(users)
    {
        Plans = plans;
        Sessions = sessions;
    }

    // FIXME
    public async Task<IResult> GetListSessions(HttpContext context, int planId) {
        return Results.Ok(Sessions.GetAll(planId));
    }

    // FIXME
    public async Task<IResult> GetSession(HttpContext context, int id) {
        return Results.Ok(Sessions.Get(id));
    }

    public class PostMarkSessionRequest
    {
        public required int Id { get; set; }
        public required int PagesRead { get; set; }
    }

    public class PostMarkSessionResponse
    {

    }

    // FIXME
    public async Task<IResult> PostMarkSession(HttpContext context) {
        var req = await ReadJson<PostMarkSessionRequest>(context.Request);

        ReadingSession session = Sessions.Get(req.Id);
        BookPlan plan = Plans.FindPlan(session.PlanId);

        plan.MarkReadingSession(session, req.PagesRead);

        return Results.Ok(new PostMarkSessionNoAuthResponse {});
    }

    public class GetSessionNoAuthResponse
    {
        public int Id { get; set; }
        public string Date { get; set; } = null!;
        public int Goal { get; set; }
        public int Actual { get; set; }
    }

    public IResult GetSessionNoAuth(HttpContext context, int id, string token)
    {
        try {
            ReadingSession session = Sessions.Get(id);

            if (token != session.GenerateToken())
                return Results.BadRequest(new ErrorResponse { Error = "Bad token" });

            return Results.Ok(new GetSessionNoAuthResponse {
                Id = session.Id,
                Date = session.Date,
                Goal = session.Goal,
                Actual = session.Actual
            });
        }
        catch (KeyNotFoundException) {
            return Results.BadRequest(new ErrorResponse { Error = "Session not found" });
        }
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

        BookPlan? plan = Plans.FindPlan(session.PlanId);

        if (plan is null)
            return Results.BadRequest(new ErrorResponse { Error = "Plan not found" });

        plan.MarkReadingSession(session, req.PagesRead);

        return Results.Ok(new PostMarkSessionNoAuthResponse{});
    }

    public override void Map(WebApplication app)
    {
        app.MapGet("/sessions/list/{planId}", GetListSessions).RequireAuthorization("Users");
        app.MapGet("/sessions/get/{id}", GetSession).RequireAuthorization("Users");
        app.MapPost("/sessions/mark", PostMarkSession).RequireAuthorization("Users");
        app.MapPost("/sessions/get_noauth", GetSessionNoAuth);
        app.MapPost("/sessions/mark_noauth", PostMarkSessionNoAuth);
    }
}
