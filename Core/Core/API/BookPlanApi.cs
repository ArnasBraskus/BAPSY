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
        public List<int> Ids;
    }

    public IResult ListBookPlans(HttpContext context)
    {
        User? user = CheckAuth(context);

        if (user is null)
            return BadAuth;

        List<int> ids = Plans.FindPlanByUser(user.Id);

        return Results.Ok(new ListBookPlansResponse { Ids = ids });
    }
    
    private class AddBookPlanRequest {
        public required string Title { get; set; } = null!;
        public required string Author { get; set; } = null!;
        public required int Pages { get; set; }
        public required string Deadline { get; set; } = null!;
        public required bool[] Weekdays { get; set; }
        public required string TimeOfDay { get; set; } = null!;
    };

    public async Task<IResult> PostAddBookPlan(HttpRequest request, HttpContext context)
    {
        User? user = CheckAuth(context);

        if (user is null)
            return BadAuth;

        var req = await ReadJson<AddBookPlanRequest>(request);

        if (req is null)
            return BadJson;

        if (!Plans.AddPlan(user, req.Deadline, Weekdays.ToBitField(req.Weekdays), req.TimeOfDay, 0, req.Title, req.Author, req.Pages))
            return Results.BadRequest(new ErrorResponse { Error = "Failed to add plan." });

        return Results.Ok(new { });
    }

    public IResult GetBookPlan(HttpContext context, int id)
    {
        User? user = CheckAuth(context);

        if (user is null)
            return BadAuth;

        BookPlan? plan = Plans.FindPlan(id);

        if (plan == null || plan.UserId != user.Id)
            return Results.BadRequest(new ErrorResponse { Error = "Plan not found." });

        return Results.Ok(new AddBookPlanRequest
        {
            Author = plan.Author,
            Title = plan.Title,
            PageCount = plan.PageCount,
            Deadline = plan.DeadLine,
            Weekdays = Weekdays.FromBitField(plan.DayOfWeek),
            TimeOfDay = plan.timeOfDay
        });
    }

    private class RemoveBookPlanRequest
    {
        public int Id { get; set; }
    };

    public async Task<IResult> PostRemoveBookPlan(HttpRequest request, HttpContext context)
    {
        User? user = CheckAuth(context);

        if (user is null)
            return BadAuth;

        var req = await ReadJson<RemoveBookPlanRequest>(request);

        if (req is null)
            return BadJson;

        BookPlan? plan = Plans.FindPlan(req.Id);

        if (plan == null || plan.UserId != user.Id)
            return Results.BadRequest(new { Error = "Plan not found." });

        if (!Plans.DeletePlan(plan.Id))
            return Results.BadRequest(new { Error = "Failed to remove plan." });

        return Results.Ok(new { });
    }

    private class EditBookPlanRequest {
        public required int Id { get; set; }
        public required string Title { get; set; } = null!;
        public required string Author { get; set; } = null!;
        public required int Pages { get; set; }
        public required string Deadline { get; set; } = null!;
        public required bool[] Weekdays { get; set; }
        public required string TimeOfDay { get; set; } = null!;
    };

    public async Task<IResult> PostEditBookPlan(HttpRequest request, HttpContext context)
    {
        User? user = CheckAuth(context);

        if (user is null)
            return BadAuth;

        var data = await ReadJson<EditBookPlanRequest>(request);

        if (data is null)
            return BadJson;

        BookPlan? plan = Plans.FindPlan(data.Id);

        if (plan == null || plan.UserId != user.Id)
            return Results.BadRequest(new { Error = "Plan not found." });

        if (!Plans.UpdatePlan(plan.Id, data.Deadline, Weekdays.ToBitField(data.Weekdays), data.TimeOfDay, data.Title, data.Author, data.Pages))
            return Results.BadRequest(new { Error = "Failed to update plan." });

        return Results.Ok(new { });
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