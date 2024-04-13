using Microsoft.AspNetCore.Http.HttpResults;

public class PlanApiTests
{
    [Theory]
    [MemberData(nameof(PlansTestsUtils.GetTestIdsFromPopulatedDb), MemberType = typeof(PlansTestsUtils))]
    public void Test_ListBookPlans_ReturnsOk(string email, int id, string deadline, bool[] days, string time, int pagesPerDay, string title, string author, int pgcount)
    {
        HttpContext context = ApiTestUtils.FakeContext(email);

        Database database = TestUtils.CreateDatabase();

        Users users = UserTestsUtils.CreatePopulated(database);

        Plans plans = new Plans(database);
        User? usr = users.FindUser(email);

        plans.AddPlan(usr, deadline, Weekdays.ToBitField(days), time, pagesPerDay, title, author, pgcount);

        BookPlanApi bookPlanApi = new BookPlanApi(users, plans);

        var result = bookPlanApi.ListBookPlans(context);

        Assert.IsType<Ok<BookPlanApi.ListBookPlansResponse>>(result);

        var response = (Ok<BookPlanApi.ListBookPlansResponse>)result;

        var actual = response.Value;

        Assert.NotNull(actual);
        Assert.Contains(1, actual.Ids);
    }

    [Fact]
    public void Test_ListBookPlansButEmty_ReturnsBadRequest()
    {
        var email = UserTestsUtils.GetFirstUserEmail();

        HttpContext context = ApiTestUtils.FakeContext(email);
        Database database = TestUtils.CreateDatabase();

        Users users = new Users(database);

        Plans plans = new Plans(database);

        BookPlanApi bookPlanApi = new BookPlanApi(users, plans);

        var result = bookPlanApi.ListBookPlans(context);

        Assert.IsType<BadRequest<ApiBase.ErrorResponse>>(result);
    }

    [Fact]
    public void Test_ListBookPlans_WithValidUser_ReturnsBadResult()
    {
        Database db = TestUtils.CreateDatabase();

        var users = UserTestsUtils.CreatePopulated(db);
        var plans = new Plans(db);

        var bookPlanApi = new BookPlanApi(users, plans);
        var context = ApiTestUtils.FakeContext();

        var result = bookPlanApi.ListBookPlans(context);

        Assert.IsType<BadRequest<ApiBase.ErrorResponse>>(result);
    }

    [Theory]
    [MemberData(nameof(PlansTestsUtils.GetTestIdsFromPopulatedDb), MemberType = typeof(PlansTestsUtils))]
    public void Test_Good_GetPlan_ReturnsOk(string email, int id, string deadline, bool[] days, string time, int pagesPerDay, string title, string author, int pgcount)
    {
        HttpContext context = ApiTestUtils.FakeContext(email);

        Database database = TestUtils.CreateDatabase();

        Users users = UserTestsUtils.CreatePopulated(database);

        Plans plans = new Plans(database);
        User? usr = users.FindUser(email);

        plans.AddPlan(usr, deadline, Weekdays.ToBitField(days), time, pagesPerDay, title, author, pgcount);

        BookPlanApi bookPlanApi = new BookPlanApi(users, plans);

        var result = bookPlanApi.GetBookPlan(context, 1);

        Assert.IsType<Ok<BookPlanApi.GetBookPlanResponse>>(result);
        var response = (Ok<BookPlanApi.GetBookPlanResponse>)result;
        var actual = (BookPlanApi.GetBookPlanResponse?)response.Value;
        Assert.NotNull(actual);
        Assert.Equal(title, actual.Title);
        Assert.Equal(author, actual.Author);
        Assert.Equal(pgcount, actual.PageCount);
        Assert.Equal(days, actual.Weekdays);
        Assert.Equal(deadline, actual.Deadline);
        Assert.Equal(time, actual.TimeOfDay);
    }

    [Fact]
    public void Test_NoUser_GetPlan_ReturnsBadAuth()
    {
        var email = UserTestsUtils.GetFirstUserEmail();

        HttpContext context = ApiTestUtils.FakeContext(email);

        Users users = UserTestsUtils.CreateEmpty();
        Plans plans = PlansTestsUtils.CreateEmpty();

        BookPlanApi bookPlanApi = new BookPlanApi(users, plans);

        var result = bookPlanApi.GetBookPlan(context, 1);

        Assert.IsType<BadRequest<ApiBase.ErrorResponse>>(result);
    }

    [Fact]
    public void Test_NoPlan_GetPlan_ReturnsBadAuth()
    {
        var email = UserTestsUtils.GetFirstUserEmail();

        HttpContext context = ApiTestUtils.FakeContext(email);

        Users users = UserTestsUtils.CreatePopulated();
        Plans plans = PlansTestsUtils.CreateEmpty();

        BookPlanApi bookPlanApi = new BookPlanApi(users, plans);

        var result = bookPlanApi.GetBookPlan(context, 1);

        Assert.IsType<BadRequest<ApiBase.ErrorResponse>>(result);
    }

    [Fact]
    public async Task Test_NoUser_PostAddBookPlan_ReturnsBadReq()
    {
        var email = UserTestsUtils.GetFirstUserEmail();

        Users users = UserTestsUtils.CreateEmpty();
        Plans plans = PlansTestsUtils.CreateEmpty();

        BookPlanApi bookPlanApi = new BookPlanApi(users, plans);

        HttpContext context = ApiTestUtils.FakeContext();

        var result = await bookPlanApi.PostAddBookPlan(context);

        Assert.IsType<BadRequest<ApiBase.ErrorResponse>>(result);
    }

    [Fact]
    public async Task Test_PostAddBookPlan_ReturnsBadReq()
    {
        var email = UserTestsUtils.GetFirstUserEmail();

        Users users = UserTestsUtils.CreatePopulated();
        Plans plans = PlansTestsUtils.CreateEmpty();

        BookPlanApi bookPlanApi = new BookPlanApi(users, plans);

        HttpContext context = ApiTestUtils.FakeContext(email);

        var result = await bookPlanApi.PostAddBookPlan(context);

        Assert.IsType<BadRequest<ApiBase.ErrorResponse>>(result);
    }

    [Fact]
    private async Task Test_NoUsr_RemoveBookPlanRequest_ReturnsBadReq()
    {
        var email = UserTestsUtils.GetFirstUserEmail();

        Users users = UserTestsUtils.CreateEmpty();
        Plans plans = PlansTestsUtils.CreateEmpty();

        BookPlanApi bookPlanApi = new BookPlanApi(users, plans);

        HttpContext context = ApiTestUtils.FakeContext(email);

        var result = await bookPlanApi.PostRemoveBookPlan(context);

        Assert.IsType<BadRequest<ApiBase.ErrorResponse>>(result);
    }

    [Fact]
    public async Task Test_RemoveBookPlanRequest_ReturnsBadReq()
    {
        var email = UserTestsUtils.GetFirstUserEmail();

        Users users = UserTestsUtils.CreatePopulated();
        Plans plans = PlansTestsUtils.CreateEmpty();

        BookPlanApi bookPlanApi = new BookPlanApi(users, plans);

        HttpContext context = ApiTestUtils.FakeContext(email);

        var result = await bookPlanApi.PostRemoveBookPlan(context);

        Assert.IsType<BadRequest<ApiBase.ErrorResponse>>(result);
    }

    [Fact]
    public async Task Test_NoUser_PostEditBookPlan_ReturnsBadReq()

    {
        var email = UserTestsUtils.GetFirstUserEmail();

        Users users = UserTestsUtils.CreateEmpty();
        Plans plans = PlansTestsUtils.CreateEmpty();

        BookPlanApi bookPlanApi = new BookPlanApi(users, plans);

        HttpContext context = ApiTestUtils.FakeContext(email);

        var result = await bookPlanApi.PostEditBookPlan(context);

        Assert.IsType<BadRequest<ApiBase.ErrorResponse>>(result);
    }

    [Fact]
    public async Task Test_PostEditBookPlan_ReturnsBadReq()

    {
        var email = UserTestsUtils.GetFirstUserEmail();

        Users users = UserTestsUtils.CreatePopulated();
        Plans plans = PlansTestsUtils.CreateEmpty();

        BookPlanApi bookPlanApi = new BookPlanApi(users, plans);

        HttpContext context = ApiTestUtils.FakeContext(email);

        var result = await bookPlanApi.PostEditBookPlan(context);

        Assert.IsType<BadRequest<ApiBase.ErrorResponse>>(result);
    }
}
