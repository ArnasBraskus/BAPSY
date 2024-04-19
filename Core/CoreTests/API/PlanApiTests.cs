using Microsoft.AspNetCore.Http.HttpResults;

public class PlanApiTests
{
    private static BookPlanApi CreateBookPlanApi(bool populatePlans = true)
    {
        Database database = TestUtils.CreateDatabase();

        Users users = UserTestsUtils.CreatePopulated(database);
        Plans plans = populatePlans ?
            PlansTestsUtils.CreatePopulated(database, users) : new Plans(database);
        ReadingSessions sessions = new ReadingSessions(database);

        var dateTimeProvider = TestUtils.CreateDateTimeMock(new DateTime(2024, 4, 10));

        return new BookPlanApi(users, plans, sessions, dateTimeProvider);
    }

    [Theory]
    [MemberData(nameof(PlansTestsUtils.GetTestIdsFromPopulatedDb), MemberType = typeof(PlansTestsUtils))]
    public void Test_ListBookPlans_ReturnsOk(string email, int id, string deadline, bool[] days, string time, int pagesPerDay, string title, string author, int pgcount)
    {
        HttpContext context = ApiTestUtils.FakeContext(email);

        Database database = TestUtils.CreateDatabase();

        Users users = UserTestsUtils.CreatePopulated(database);
        ReadingSessions sessions = new ReadingSessions(database);

        Plans plans = new Plans(database);
        User? usr = users.FindUser(email);

        plans.AddPlan(usr, deadline, Weekdays.ToBitField(days), time, pagesPerDay, title, author, pgcount);

        BookPlanApi bookPlanApi = new BookPlanApi(users, plans, sessions);

        var result = bookPlanApi.ListBookPlans(context);

        Assert.IsType<Ok<BookPlanApi.ListBookPlansResponse>>(result);

        var response = (Ok<BookPlanApi.ListBookPlansResponse>)result;

        var actual = response.Value;

        Assert.NotNull(actual);
        Assert.Contains(1, actual.Ids);
    }

    [Theory]
    [MemberData(nameof(PlansTestsUtils.GetTestPlansWithIds), MemberType = typeof(PlansTestsUtils))]
    public void Test_Good_GetPlan_ReturnsOk(int id, int userId, string deadline, bool[] days, string time, int pagesPerDay, string title, string author, int pgcount)
    {
        var EMAIL = UserTestsUtils.GetUserEmail(userId);

        HttpContext context = ApiTestUtils.FakeContext(EMAIL);
        BookPlanApi bookPlanApi = CreateBookPlanApi();

        var result = bookPlanApi.GetBookPlan(context, id);

        Assert.IsType<Ok<BookPlanApi.GetBookPlanResponse>>(result);
        var response = (Ok<BookPlanApi.GetBookPlanResponse>)result;
        var actual = (BookPlanApi.GetBookPlanResponse?)response.Value;
        Assert.NotNull(actual);
        Assert.Equal(title, actual.Title);
        Assert.Equal(author, actual.Author);
        Assert.Equal(pgcount, actual.Pages);
        Assert.Equal(days, actual.Weekdays);
        Assert.Equal(deadline, actual.Deadline);
        Assert.Equal(time, actual.TimeOfDay);
    }

    [Fact]
    public void Test_UserDoesntHavePlan_GetPlan_ReturnsBadRequest()
    {
        var EMAIL = UserTestsUtils.GetFirstUserEmail();
        var PLAN_ID = 1;

        HttpContext context = ApiTestUtils.FakeContext(EMAIL);
        BookPlanApi bookPlanApi = CreateBookPlanApi(false);

        var result = bookPlanApi.GetBookPlan(context, PLAN_ID);

        Assert.IsType<BadRequest<ApiBase.ErrorResponse>>(result);
    }

    [Fact]
    public void Test_PlanDoesntExist_GetPlan_ReturnsBadAuth()
    {
        var EMAIL = UserTestsUtils.GetFirstUserEmail();
        var PLAN_ID = 99;

        HttpContext context = ApiTestUtils.FakeContext(EMAIL);

        Database db = TestUtils.CreateDatabase();
        BookPlanApi bookPlanApi = CreateBookPlanApi(false);

        var result = bookPlanApi.GetBookPlan(context, PLAN_ID);

        Assert.IsType<BadRequest<ApiBase.ErrorResponse>>(result);
    }

    [Fact]
    public async Task Test_DeadlineIsPassed_PostAddBookPlan_ReturnsBadRequest()
    {
        var EMAIL = UserTestsUtils.GetFirstUserEmail();
        var DATA = "{\"title\": \"Title\", \"author\": \"Arlen Anmore\", \"pages\": 74, \"deadline\": \"2024-04-09\", \"weekdays\": [true, true, true, true, true, false, false], \"timeofday\": \"10:35\"}";

        HttpContext context = ApiTestUtils.FakeContext(EMAIL, DATA);
        BookPlanApi bookPlanApi = CreateBookPlanApi(false);

        var result = await bookPlanApi.PostAddBookPlan(context);

        Assert.IsType<BadRequest<ApiBase.ErrorResponse>>(result);
    }

    [Fact]
    public async Task Test_ValidRequest_PostAddBookPlan_ReturnsOk()
    {
        var EMAIL = UserTestsUtils.GetFirstUserEmail();
        var DATA = "{\"title\": \"Touchy Feely\", \"author\": \"Arlen Anmore\", \"pages\": 74, \"deadline\": \"2024-04-20\", \"weekdays\": [true, true, true, true, true, false, false], \"timeofday\": \"10:35\"}";

        HttpContext context = ApiTestUtils.FakeContext(EMAIL, DATA);
        BookPlanApi bookPlanApi = CreateBookPlanApi(false);

        var result = await bookPlanApi.PostAddBookPlan(context);

        Assert.IsType<Ok<BookPlanApi.AddBookPlanResponse>>(result);
    }

    [Fact]
    public async Task Test_DeadlineIsPassed_PostEditBookPlan_ReturnsBadRequest()
    {
        var EMAIL = UserTestsUtils.TestUsers1[2].Item1;
        var DATA = "{\"id\": 1, \"title\": \"Touchy Feely\", \"author\": \"Arlen Anmore\", \"pages\": 74, \"deadline\": \"2024-04-01\", \"weekdays\": [true, true, true, true, true, false, false], \"timeofday\": \"10:35\"}";

        HttpContext context = ApiTestUtils.FakeContext(EMAIL, DATA);

        Database database = TestUtils.CreateDatabase();

        Users users = UserTestsUtils.CreatePopulated(database);
        Plans plans = PlansTestsUtils.CreatePopulated(database, users);
        ReadingSessions sessions = new ReadingSessions(database);

        BookPlanApi bookPlanApi = new BookPlanApi(users, plans, sessions);

        var result = await bookPlanApi.PostEditBookPlan(context);

        Assert.IsType<BadRequest<ApiBase.ErrorResponse>>(result);
    }

    [Fact]
    public async Task Test_ValidRequest_PostEditBookPlan_ReturnsOk()
    {
        var EMAIL = UserTestsUtils.TestUsers1[2].Item1;
        var DATA = "{\"id\": 1, \"title\": \"Touchy Feely\", \"author\": \"Arlen Anmore\", \"pages\": 74, \"deadline\": \"2024-04-24\", \"weekdays\": [true, true, true, true, true, false, false], \"timeofday\": \"10:35\"}";

        HttpContext context = ApiTestUtils.FakeContext(EMAIL, DATA);
        BookPlanApi bookPlanApi = CreateBookPlanApi();

        var result = await bookPlanApi.PostEditBookPlan(context);

        Assert.IsType<Ok<BookPlanApi.EditBookPlanResponse>>(result);
    }

    [Fact]
    public async Task Test_InvalidPlan_PostEditBookPlan_ReturnsBadRequest()
    {
        var EMAIL = UserTestsUtils.TestUsers1[2].Item1;
        var DATA = "{\"id\": 12, \"title\": \"Touchy Feely\", \"author\": \"Arlen Anmore\", \"pages\": 74, \"deadline\": \"2024-04-11\", \"weekdays\": [true, true, true, true, true, false, false], \"timeofday\": \"10:35\"}";

        HttpContext context = ApiTestUtils.FakeContext(EMAIL, DATA);
        BookPlanApi bookPlanApi = CreateBookPlanApi();

        var result = await bookPlanApi.PostEditBookPlan(context);

        Assert.IsType<BadRequest<ApiBase.ErrorResponse>>(result);
    }

    [Fact]
    public async Task Test_InvalidPlanDetails_PostEditBookPlan_ReturnsBadRequest()
    {
        var EMAIL = UserTestsUtils.TestUsers1[2].Item1;
        var DATA = "{\"id\": 1, \"title\": \"aaa\", \"author\": \"Arlen Anmore\", \"pages\": -1, \"deadline\": \"2024-04-11\", \"weekdays\": [true, true, true, true, true, false, false], \"timeofday\": \"10:35\"}";

        HttpContext context = ApiTestUtils.FakeContext(EMAIL, DATA);
        BookPlanApi bookPlanApi = CreateBookPlanApi();

        var result = await bookPlanApi.PostEditBookPlan(context);

        Assert.IsType<BadRequest<ApiBase.ErrorResponse>>(result);
    }

    [Fact]
    public async Task Test_ValidIdButUserDoesntHavePlan_RemovePlan_ReturnsBadRequest()
    {
        var EMAIL = UserTestsUtils.GetFirstUserEmail();
        var DATA = "{\"id\": 1}";

        HttpContext context = ApiTestUtils.FakeContext(EMAIL, DATA);
        BookPlanApi bookPlanApi = CreateBookPlanApi();

        var result = await bookPlanApi.PostRemoveBookPlan(context);

        Assert.IsType<BadRequest<ApiBase.ErrorResponse>>(result);
    }

    [Fact]
    public async Task Test_InvalidId_RemovePlan_ReturnsBadRequest()
    {
        var EMAIL = UserTestsUtils.GetFirstUserEmail();
        var DATA = "{\"id\": 12}";

        HttpContext context = ApiTestUtils.FakeContext(EMAIL, DATA);
        BookPlanApi bookPlanApi = CreateBookPlanApi();

        var result = await bookPlanApi.PostRemoveBookPlan(context);

        Assert.IsType<BadRequest<ApiBase.ErrorResponse>>(result);
    }

    [Fact]
    public async Task Test_ValidId_RemovePlan_ReturnsOk()
    {
        var EMAIL = UserTestsUtils.TestUsers1[2].Item1;
        var DATA = "{\"id\": 1}";

        HttpContext context = ApiTestUtils.FakeContext(EMAIL, DATA);
        BookPlanApi bookPlanApi = CreateBookPlanApi();

        var result = await bookPlanApi.PostRemoveBookPlan(context);

        Assert.IsType<Ok<BookPlanApi.RemoveBookPlanResponse>>(result);
    }

}
