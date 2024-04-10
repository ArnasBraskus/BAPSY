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

    /*[Fact]
    public async Task Test_ValidRequest_PostAddBookPlan_ReturnsOk()
    {
        var email = UserTestsUtils.GetFirstUserEmail();

        HttpContext context = ApiTestUtils.FakeContext(email);

        Users users = UserTestsUtils.CreatePopulated();

        Plans plans = PlansTestsUtils.CreateEmpty();

        BookPlanApi bookPlanApi = new BookPlanApi(users, plans);

        var addBookPlanRequest = new BookPlanApi.AddBookPlanRequest
        {
            Author = "Author",
            Title = "Title",
            Pages = 100,
            Deadline = "6-11-2024",
            Weekdays = Weekdays.FromBitField(0b1010101),
            TimeOfDay = "14:14"
        };

        var request = ApiTestUtils.CreateFakeRequest(addBookPlanRequest);

        var result = await bookPlanApi.PostAddBookPlan(request, context);

        Assert.IsType<Ok<object>>(result);
    }*/

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

        Assert.IsType<Ok<BookPlanApi.AddBookPlanRequest>>(result);
        var response = (Ok<BookPlanApi.AddBookPlanRequest>)result;
        var actual = (BookPlanApi.AddBookPlanRequest?)response.Value;
        Assert.NotNull(actual);
        Assert.Equal(title, actual.Title);
        Assert.Equal(author, actual.Author);
        Assert.Equal(pgcount, actual.Pages);
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
}