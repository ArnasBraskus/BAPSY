namespace Core;

public class SessionApi : ApiBase
{
    private readonly Plans Plans;
    private readonly ReadingSessions Sessions;

    public SessionApi(Users users, Plans plans, ReadingSessions sessions) : base(users)
    {
        Plans = plans;
        Sessions = sessions;
    }
	internal class ListReadingSessionsResponse
	{
		public List<ReadingSession> Sessions { get; set; } = null!;
	}

	// FIXME
	public IResult GetListSessions(HttpContext context, int planId)

    {
		
		if (Plans.FindPlan(planId) == null)
		{
			return Results.BadRequest( "Plan not found." );
		}

		List<ReadingSession> sessions = Sessions.GetAll(planId);

		return Results.Ok(new ListReadingSessionsResponse { Sessions = sessions });	
    }

    internal class GetSessionResponse
    {
        public int Id { get; set; }
        public int PlanId { get; set; } 
        public string Date { get; set; }
        public int Goal { get; set; }
        public int _Actual { get; set; }
        public int IsCompleted { get; set; }
	}

    // FIXME 
    public IResult GetSession(HttpContext context, int id)
    {
		try
		{	

			ReadingSession session = Sessions.Get(id);

            return Results.Ok(new GetSessionResponse
            {
                Id = id,
                PlanId = session.Id,
                Date = session.Date,
                Goal = session.Goal,
                _Actual = session.Actual,
                IsCompleted = session.IsCompleted
            }) ;
		}
		catch (KeyNotFoundException)
		{
			return Results.NotFound("Reading session not found.");
		}
	}

    internal class PostMarkSessionRequest
    {
        public required int Id { get; set; }
        public required int PagesRead { get; set; }
    }


    // FIXME
    public async Task<IResult> PostMarkSession(HttpContext context)
    {
        var req = await ReadJson<PostMarkSessionRequest>(context.Request).ConfigureAwait(false);

        ReadingSession session = Sessions.Get(req.Id);
        BookPlan plan = Plans.FindPlan(session.PlanId);

        plan.MarkReadingSession(session, req.PagesRead);

        return Results.Ok();
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
        try
        {
            ReadingSession session = Sessions.Get(id);

            if (token != session.GenerateToken())
                return Results.BadRequest(new ErrorResponse { Error = "Bad token" });

            return Results.Ok(new GetSessionNoAuthResponse
            {
                Id = session.Id,
                Date = session.Date,
                Goal = session.Goal,
                Actual = session.Actual
            });
        }
        catch (KeyNotFoundException)
        {
            return Results.BadRequest(new ErrorResponse { Error = "Session not found" });
        }
    }

    public class PostMarkSessionNoAuthRequest
    {
        public required string Token { get; set; }
        public required int SessionId { get; set; }
        public required int PagesRead { get; set; }
    }


    public async Task<IResult> PostMarkSessionNoAuth(HttpRequest request)
    {
        var req = await ReadJson<PostMarkSessionNoAuthRequest>(request).ConfigureAwait(false);

        ReadingSession session = Sessions.Get(req.SessionId);

        if (req.Token != session.GenerateToken())
            return Results.BadRequest(new ErrorResponse { Error = "Bad token" });

        BookPlan? plan = Plans.FindPlan(session.PlanId);

        if (plan is null)
            return Results.BadRequest(new ErrorResponse { Error = "Plan not found" });

        plan.MarkReadingSession(session, req.PagesRead);

        return Results.Ok();
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
