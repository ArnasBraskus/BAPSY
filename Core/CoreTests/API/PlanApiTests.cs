using Microsoft.AspNetCore.Http;
using Xunit;
using Moq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using static ApiBase;

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

    [Fact]
    public async Task Test_BadJson_PostAddBookPlan_ReturnsBadRequest()
    {
        var email = UserTestsUtils.GetFirstUserEmail();
        var data = "{\"title\": \"\", \"author\": \"Arlen Anmore\", \"pageCount\": 74, \"deadline\": \"2024-04-11\", \"weekdays\": [true, true, true, true, true, false, false], \"timeofday\": \"10:35\"";

        HttpContext context = ApiTestUtils.FakeContext(email, data);

        Database db = TestUtils.CreateDatabase();

        Users users = UserTestsUtils.CreatePopulated(db);

        Plans plans = new Plans(db);

        BookPlanApi bookPlanApi = new BookPlanApi(users, plans);

        var result = await bookPlanApi.PostAddBookPlan(context);

        Assert.IsType<BadRequest<ApiBase.ErrorResponse>>(result);
    }

    [Fact]
    public async Task Test_EmptyTitle_PostAddBookPlan_ReturnsBadRequest()
    {
        var email = UserTestsUtils.GetFirstUserEmail();
        var data = "{\"title\": \"\", \"author\": \"Arlen Anmore\", \"pageCount\": 74, \"deadline\": \"2024-04-11\", \"weekdays\": [true, true, true, true, true, false, false], \"timeofday\": \"10:35\"}";

        HttpContext context = ApiTestUtils.FakeContext(email, data);

        Database db = TestUtils.CreateDatabase();

        Users users = UserTestsUtils.CreatePopulated(db);

        Plans plans = new Plans(db);

        BookPlanApi bookPlanApi = new BookPlanApi(users, plans);

        var result = await bookPlanApi.PostAddBookPlan(context);

        Assert.IsType<BadRequest<ApiBase.ErrorResponse>>(result);
    }

    [Fact]
    public async Task Test_ValidRequest_PostAddBookPlan_ReturnsOk()
    {
        var email = UserTestsUtils.GetFirstUserEmail();
        var data = "{\"title\": \"Touchy Feely\", \"author\": \"Arlen Anmore\", \"pageCount\": 74, \"deadline\": \"2024-04-11\", \"weekdays\": [true, true, true, true, true, false, false], \"timeofday\": \"10:35\"}";

        HttpContext context = ApiTestUtils.FakeContext(email, data);

        Database db = TestUtils.CreateDatabase();

        Users users = UserTestsUtils.CreatePopulated(db);

        Plans plans = new Plans(db);

        BookPlanApi bookPlanApi = new BookPlanApi(users, plans);

        var result = await bookPlanApi.PostAddBookPlan(context);

        Assert.IsType<Ok<BookPlanApi.AddBookPlanResponse>>(result);
    }

    [Fact]
    public async Task Test_ValidRequest_PostEditBookPlan_ReturnsOk()
    {
        var email = UserTestsUtils.TestUsers1[2].Item1;
        var data = "{\"id\": 1, \"title\": \"Touchy Feely\", \"author\": \"Arlen Anmore\", \"pageCount\": 74, \"deadline\": \"2024-04-11\", \"weekdays\": [true, true, true, true, true, false, false], \"timeofday\": \"10:35\"}";

        HttpContext context = ApiTestUtils.FakeContext(email, data);

        Database db = TestUtils.CreateDatabase();

        Users users = UserTestsUtils.CreatePopulated(db);
        Plans plans = PlansTestsUtils.CreatePopulated(db, users);

        BookPlanApi bookPlanApi = new BookPlanApi(users, plans);

        var result = await bookPlanApi.PostEditBookPlan(context);

        /*var response = (BadRequest<ApiBase.ErrorResponse>)result;
        var actual = (ApiBase.ErrorResponse?)response.Value;
        Console.WriteLine(actual.Error); */

        Assert.IsType<Ok<BookPlanApi.EditBookPlanResponse>>(result);
    }

    [Fact]
    public async Task Test_InvalidPlan_PostEditBookPlan_ReturnsBadRequest()
    {
        var email = UserTestsUtils.TestUsers1[2].Item1;
        var data = "{\"id\": 12, \"title\": \"Touchy Feely\", \"author\": \"Arlen Anmore\", \"pageCount\": 74, \"deadline\": \"2024-04-11\", \"weekdays\": [true, true, true, true, true, false, false], \"timeofday\": \"10:35\"}";

        HttpContext context = ApiTestUtils.FakeContext(email, data);

        Database db = TestUtils.CreateDatabase();

        Users users = UserTestsUtils.CreatePopulated(db);

        Plans plans = new Plans(db);

        BookPlanApi bookPlanApi = new BookPlanApi(users, plans);

        var result = await bookPlanApi.PostEditBookPlan(context);

        Assert.IsType<BadRequest<ApiBase.ErrorResponse>>(result);
    }

    [Fact]
    public async Task Test_InvalidAuth_PostEditBookPlan_ReturnsBadRequest()
    {
        var email = UserTestsUtils.TestUsers1[2].Item1;
        var data = "{\"id\": 1, \"author\": \"Arlen Anmore\", \"pageCount\": 100, \"deadline\": \"2024-04-11\", \"weekdays\": [true, true, true, true, true, false, false], \"timeofday\": \"10:35\"}";

        HttpContext context = ApiTestUtils.FakeContext(null, data);

        Database db = TestUtils.CreateDatabase();

        Users users = UserTestsUtils.CreatePopulated(db);
        Plans plans = PlansTestsUtils.CreatePopulated(db, users);

        BookPlanApi bookPlanApi = new BookPlanApi(users, plans);

        var result = await bookPlanApi.PostEditBookPlan(context);
        /*var response = (BadRequest<ApiBase.ErrorResponse>)result;
        var actual = (ApiBase.ErrorResponse?)response.Value;
        Console.WriteLine(actual.Error);  */
        Assert.IsType<BadRequest<ApiBase.ErrorResponse>>(result);
    }

    [Fact]
    public async Task Test_InvalidJson_PostEditBookPlan_ReturnsBadRequest()
    {
        var email = UserTestsUtils.TestUsers1[2].Item1;
        var data = "{\"id\": 1, \"author\": \"Arlen Anmore\", \"pageCount\": 100, \"deadline\": \"2024-04-11\", \"weekdays\": [true, true, true, true, true, false, false], \"timeofday\": \"10:35\"}";

        HttpContext context = ApiTestUtils.FakeContext(email, data);

        Database db = TestUtils.CreateDatabase();

        Users users = UserTestsUtils.CreatePopulated(db);
        Plans plans = PlansTestsUtils.CreatePopulated(db, users);

        BookPlanApi bookPlanApi = new BookPlanApi(users, plans);

        var result = await bookPlanApi.PostEditBookPlan(context);
        /*var response = (BadRequest<ApiBase.ErrorResponse>)result;
        var actual = (ApiBase.ErrorResponse?)response.Value;
        Console.WriteLine(actual.Error);  */
        Assert.IsType<BadRequest<ApiBase.ErrorResponse>>(result);
    }

    [Fact]
    public async Task Test_InvalidPlanDetails_PostEditBookPlan_ReturnsBadRequest()
    {
        var email = UserTestsUtils.TestUsers1[2].Item1;
        var data = "{\"id\": 1, \"title\": \"aaa\", \"author\": \"Arlen Anmore\", \"pageCount\": -1, \"deadline\": \"2024-04-11\", \"weekdays\": [true, true, true, true, true, false, false], \"timeofday\": \"10:35\"}";

        HttpContext context = ApiTestUtils.FakeContext(email, data);

        Database db = TestUtils.CreateDatabase();

        Users users = UserTestsUtils.CreatePopulated(db);
        Plans plans = PlansTestsUtils.CreatePopulated(db, users);

        BookPlanApi bookPlanApi = new BookPlanApi(users, plans);

        var result = await bookPlanApi.PostEditBookPlan(context);
        /*var response = (BadRequest<ApiBase.ErrorResponse>)result;
        var actual = (ApiBase.ErrorResponse?)response.Value;
        Console.WriteLine(actual.Error);  */
        Assert.IsType<BadRequest<ApiBase.ErrorResponse>>(result);
    }

    [Fact]
    public async Task Test_NoAuth_PostAddBookPlan_ReturnsBadRequest()
    {
        var email = UserTestsUtils.GetFirstUserEmail();
        var data = "{\"title\": \"Touchy Feely\", \"author\": \"Arlen Anmore\", \"pageCount\": 74, \"deadline\": \"2024-04-11\", \"weekdays\": [true, true, true, true, true, false, false], \"timeofday\": \"10:35\"}";

        HttpContext context = ApiTestUtils.FakeContext(null, data);

        Database db = TestUtils.CreateDatabase();

        Users users = UserTestsUtils.CreatePopulated(db);

        Plans plans = new Plans(db);

        BookPlanApi bookPlanApi = new BookPlanApi(users, plans);

        var result = await bookPlanApi.PostAddBookPlan(context);

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
        Assert.Equal(deadline, actual.Deadline);
        Assert.Equal(time, actual.TimeOfDay);
    }

    [Fact]
    public async Task Test_BadAuth_RemovePlan_ReturnsBadRequest()
    {
        var data = "{\"id\": 1}";
        var email = UserTestsUtils.GetFirstUserEmail();

        HttpContext context = ApiTestUtils.FakeContext(null, data);

        Database database = TestUtils.CreateDatabase();

        Users users = UserTestsUtils.CreatePopulated(database);
        Plans plans = PlansTestsUtils.CreatePopulated(database, users);

        BookPlanApi bookPlanApi = new BookPlanApi(users, plans);

        var result = await bookPlanApi.PostRemoveBookPlan(context);

        Assert.IsType<BadRequest<ApiBase.ErrorResponse>>(result);
    }

    [Fact]
    public async Task Test_InvalidJson_RemovePlan_ReturnsBadRequest()
    {
        var data = "{\"id\": 1";
        var email = UserTestsUtils.GetFirstUserEmail();
        HttpContext context = ApiTestUtils.FakeContext(email, data);

        Database database = TestUtils.CreateDatabase();

        Users users = UserTestsUtils.CreatePopulated(database);
        Plans plans = PlansTestsUtils.CreatePopulated(database, users);

        BookPlanApi bookPlanApi = new BookPlanApi(users, plans);

        var result = await bookPlanApi.PostRemoveBookPlan(context);

        Assert.IsType<BadRequest<ApiBase.ErrorResponse>>(result);
    }

    [Fact]
    public async Task Test_ValidIdButInvalidUser_RemovePlan_ReturnsBadRequest()
    {
        var data = "{\"id\": 1}";
        var email = UserTestsUtils.GetFirstUserEmail();
        HttpContext context = ApiTestUtils.FakeContext(email, data);

        Database database = TestUtils.CreateDatabase();

        Users users = UserTestsUtils.CreatePopulated(database);
        Plans plans = PlansTestsUtils.CreatePopulated(database, users);

        BookPlanApi bookPlanApi = new BookPlanApi(users, plans);

        var result = await bookPlanApi.PostRemoveBookPlan(context);

        Assert.IsType<BadRequest<ApiBase.ErrorResponse>>(result);
    }

    [Fact]
    public async Task Test_InvalidId_RemovePlan_ReturnsBadRequest()
    {
        var data = "{\"id\": 12}";
        var email = UserTestsUtils.GetFirstUserEmail();
        HttpContext context = ApiTestUtils.FakeContext(email, data);

        Database database = TestUtils.CreateDatabase();

        Users users = UserTestsUtils.CreatePopulated(database);
        Plans plans = PlansTestsUtils.CreatePopulated(database, users);

        BookPlanApi bookPlanApi = new BookPlanApi(users, plans);

        var result = await bookPlanApi.PostRemoveBookPlan(context);

        Assert.IsType<BadRequest<ApiBase.ErrorResponse>>(result);
    }

    [Fact]
    public async Task Test_ValidId_RemovePlan_ReturnsOk()
    {
        var data = "{\"id\": 1}";
        var email = UserTestsUtils.TestUsers1[2].Item1;
        HttpContext context = ApiTestUtils.FakeContext(email, data);

        Database database = TestUtils.CreateDatabase();

        Users users = UserTestsUtils.CreatePopulated(database);
        Plans plans = PlansTestsUtils.CreatePopulated(database, users);

        BookPlanApi bookPlanApi = new BookPlanApi(users, plans);

        var result = await bookPlanApi.PostRemoveBookPlan(context);

        Assert.IsType<Ok<BookPlanApi.RemoveBookPlanResponse>>(result);
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
}
