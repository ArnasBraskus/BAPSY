using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

public class UserApiTests
{
    [Theory]
    [MemberData(nameof(UserTestsUtils.GetTestUsersFromPopulatedDb), MemberType = typeof(UserTestsUtils))]
    public void Test_GoodAuth_GetProfile_ReturnsUserInfo(string email, string name)
    {
        HttpContext context = ApiTestUtils.FakeContext(email);

        Users users = UserTestsUtils.CreatePopulated();

        UserApi userApi = new UserApi(users);

        var result = userApi.GetUserProfile(context);

        Assert.IsType<Ok<UserApi.UserProfileResponse>>(result);

        var response = (Ok<UserApi.UserProfileResponse>)result;

        var actual = (UserApi.UserProfileResponse?)response.Value;

        Assert.NotNull(actual);
        Assert.Equal(email, actual.Email);
        Assert.Equal(name, actual.Name);
    }

    [Fact]
    public void Test_InvalidAuth_GetProfile_ReturnsBadRequest()
    {
        HttpContext context = ApiTestUtils.FakeContext();

        Users users = UserTestsUtils.CreatePopulated();

        UserApi userApi = new UserApi(users);

        var result = userApi.GetUserProfile(context);

        Assert.IsType<BadRequest<ApiBase.ErrorResponse>>(result);
    }
}