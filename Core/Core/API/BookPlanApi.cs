public class BookPlanApi : ApiBase
{
    private Users Users;
    private Plans Plans;

    public BookPlanApi(Users users, Plans plans) : base(users)
    {
        Users = users;
        Plans = plans;
    }

    public class ListBookPlansResponse
    {
        public List<int> Ids { get; set; } = null!;
    }

    public IResult ListBookPlans(HttpContext context)
    {
        User? user = CheckAuth(context);

        if (user is null)
            return BadAuth;

        List<int> ids = Plans.FindPlanByUser(user.Id);

        return Results.Ok(new ListBookPlansResponse { Ids = ids });
    }

    public class AddBookPlanRequest
    {
        public string Title { get; set; } = null!;
        public string Author { get; set; } = null!;
        public int PageCount { get; set; }
        public string Deadline { get; set; } = null!;
        public bool[] Weekdays { get; set; } = null!;
        public string TimeOfDay { get; set; } = null!;
    }

    public class AddBookPlanResponse
    {
    };

    public async Task<IResult> PostAddBookPlan(HttpContext context)
    {
        User? user = CheckAuth(context);

        if (user is null)
            return BadAuth;

        var req = await ReadJson<AddBookPlanRequest>(context.Request);

        if (req is null)
            return BadJson;

        try
        {
            Plans.AddPlan(user, req.Deadline, Weekdays.ToBitField(req.Weekdays), req.TimeOfDay, 0, req.Title, req.Author, req.PageCount);
        }
        catch (ArgumentException e)
        {
            return Results.BadRequest(new ErrorResponse { Error = e.Message });
        }

        return Results.Ok(new AddBookPlanResponse {});
    }

    public class GetBookPlanResponse
    {
        public string Title { get; set; } = null!;
        public string Author { get; set; } = null!;
        public int PageCount { get; set; }
        public string Deadline { get; set; } = null!;
        public bool[] Weekdays { get; set; } = null!;
        public string TimeOfDay { get; set; } = null!;
    }

    public IResult GetBookPlan(HttpContext context, int id)
    {
        User? user = CheckAuth(context);

        if (user is null)
            return BadAuth;

        BookPlan? plan = Plans.FindPlan(id);

        if (plan == null || plan.UserId != user.Id)
            return Results.BadRequest(new ErrorResponse { Error = "Plan not found." });

        return Results.Ok(new GetBookPlanResponse
        {
            Author = plan.Author,
            Title = plan.Title,
            PageCount = plan.PageCount,
            Deadline = plan.DeadLine,
            Weekdays = Weekdays.FromBitField(plan.DayOfWeek),
            TimeOfDay = plan.timeOfDay
        });
    }

    public class RemoveBookPlanRequest
    {
        public int Id { get; set; }
    }

    public class RemoveBookPlanResponse
    {

    }

    public async Task<IResult> PostRemoveBookPlan(HttpContext context)
    {
        User? user = CheckAuth(context);

        if (user is null)
            return BadAuth;

        var req = await ReadJson<RemoveBookPlanRequest>(context.Request);

        if (req is null)
            return BadJson;

        BookPlan? plan = Plans.FindPlan(req.Id);

        if (plan == null || plan.UserId != user.Id)
            return Results.BadRequest(new ErrorResponse { Error = "Plan not found." });

        Plans.DeletePlan(plan.Id);

        return Results.Ok(new RemoveBookPlanResponse { });
    }

    private class EditBookPlanRequest
    {
        public required int Id { get; set; }
        public required string Title { get; set; } = null!;
        public required string Author { get; set; } = null!;
        public required int PageCount { get; set; }
        public required string Deadline { get; set; } = null!;
        public required bool[] Weekdays { get; set; } = null!;
        public required string TimeOfDay { get; set; } = null!;
    };

    public class EditBookPlanResponse
    {

    };

    public async Task<IResult> PostEditBookPlan(HttpContext context)
    {
        User? user = CheckAuth(context);

        if (user is null)
            return BadAuth;

        var data = await ReadJson<EditBookPlanRequest>(context.Request);

        if (data is null)
            return BadJson;

        BookPlan? plan = Plans.FindPlan(data.Id);

        if (plan == null || plan.UserId != user.Id)
            return Results.BadRequest(new ErrorResponse { Error = "Plan not found." });

        try
        {
            Plans.UpdatePlan(plan.Id, data.Deadline, Weekdays.ToBitField(data.Weekdays), data.TimeOfDay, data.Title, data.Author, data.PageCount);
        }
        catch (ArgumentException e)
        {
            return Results.BadRequest(new ErrorResponse { Error = e.Message });
        }

        return Results.Ok(new EditBookPlanResponse { });
    }

    public override void Map(WebApplication app)
    {
        app.MapGet("/bookplan/list", ListBookPlans).RequireAuthorization("Users");
        app.MapGet("/bookplan/get/{id}", GetBookPlan).RequireAuthorization("Users");
        app.MapPost("/bookplan/add", PostAddBookPlan).RequireAuthorization("Users");
        app.MapPost("/bookplan/remove", PostRemoveBookPlan).RequireAuthorization("Users");
        app.MapPost("/bookplan/edit", PostEditBookPlan).RequireAuthorization("Users");
    }
}
