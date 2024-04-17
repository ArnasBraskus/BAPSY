using Microsoft.AspNetCore.Http.HttpResults;

public class CalendarApiTests
{
    public CalendarApi CreateCalendarApi()
    {
        Database database = TestUtils.CreateDatabase();
        Users users = UserTestsUtils.CreatePopulated(database);
        Plans plans = PlansTestsUtils.CreatePopulated(database, users);

        var dateTimeProvider = TestUtils.CreateDateTimeMock(new DateTime(2024, 04, 10));

        return new CalendarApi(users, plans, dateTimeProvider);
    }

    [Fact]
    public async Task Test_PlanDoenstExist_GetCalendarEvents_ReturnsBadRequest()
    {
        var EMAIL = UserTestsUtils.GetFirstUserEmail();
        var ID = 99;

        HttpContext context = ApiTestUtils.FakeContext(EMAIL);
        CalendarApi api = CreateCalendarApi();

        var result = await api.GetCalendarEvents(context, ID);

        Assert.IsType<BadRequest<ApiBase.ErrorResponse>>(result);
    }

    [Fact]
    public async Task Test_PlanExistButNotForThisUser_GetCalendarEvents_ReturnsBadRequest()
    {
        var EMAIL = UserTestsUtils.GetFirstUserEmail();
        var ID = 1;

        HttpContext context = ApiTestUtils.FakeContext(EMAIL);
        CalendarApi api = CreateCalendarApi();

        var result = await api.GetCalendarEvents(context, ID);

        Assert.IsType<BadRequest<ApiBase.ErrorResponse>>(result);
    }

    [Fact]
    public async Task Test_PlanExistsForUser_GetCalendarEvents_ReturnsCalendarEvents()
    {
        var EMAIL = UserTestsUtils.GetUserEmail(6);
        var ID = 8;

        HttpContext context = ApiTestUtils.FakeContext(EMAIL);
        CalendarApi api = CreateCalendarApi();

        var result = await api.GetCalendarEvents(context, ID);

        Assert.IsType<Ok<List<ReadingEvent>>>(result);
    }

    [Fact]
    public async Task Test_PlanDoenstExist_ExportCalendar_ReturnsBadRequest()
    {
        var EMAIL = UserTestsUtils.GetFirstUserEmail();
        var ID = 99;

        HttpContext context = ApiTestUtils.FakeContext(EMAIL);
        CalendarApi api = CreateCalendarApi();

        var result = await api.GetExportCalendar(context, ID);

        Assert.IsType<BadRequest<ApiBase.ErrorResponse>>(result);
    }

    [Fact]
    public async Task Test_PlanExistsForUser_ExportCalendar_ReturnsSerializedCalendar()
    {
        var EMAIL = UserTestsUtils.GetUserEmail(6);
        var ID = 8;

        HttpContext context = ApiTestUtils.FakeContext(EMAIL);
        CalendarApi api = CreateCalendarApi();

        var result = await api.GetExportCalendar(context, ID);

        Assert.IsType<FileContentHttpResult>(result);
    }
}
