using Moq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Google.Apis.Auth;

public class AuthApiTests {
    public static readonly string TestGoogleToken = "GOOGLE_TOKEN";
    public static readonly string TestGoogleTokenBad = "GOOGLE_TOKEN_BAD";
    public static readonly string TestEmail = UserTestsUtils.GetFirstUserEmail();
    public static readonly string TestName = UserTestsUtils.GetFirstUserName();

    public static Auth CreateAuth(string? testEmail = null, string? testName = null) {
        return new Auth(AuthTests.TestSecretKey, AuthTests.TestIssuer, CreateGoogleTokenValidatorMock(testEmail ?? TestEmail, testName ?? TestName));
    }

    private static GoogleTokenValidator CreateGoogleTokenValidatorMock(string email, string name) {
        var mock = new Mock<GoogleTokenValidator>();

        mock.Setup(l => l.ValidateToken(TestGoogleToken)).Returns(new GoogleJsonWebSignature.Payload { EmailVerified = true, Email = email, Name = name});

        return mock.Object;
    }

    [Fact]
    public void Test_EmptyDb_PostAuthGoogleWithGoodToken_ServerReturnsToken() {
        HttpContext context = ApiTestUtils.FakeContext(TestEmail, $"{{\"jwttoken\": \"{TestGoogleToken}\"}}");

        Auth auth = CreateAuth();
        Users users = UserTestsUtils.CreateEmpty();

        AuthApi authApi = new AuthApi(auth, users);

        var result = authApi.PostAuthGoogle(context.Request).Result;

        Assert.IsType<Ok<AuthApi.GoogleAuthResponse>>(result);

        var response = (Ok<AuthApi.GoogleAuthResponse>)result;

        var actual = (AuthApi.GoogleAuthResponse?)response.Value;

        Assert.NotNull(actual);
        Assert.NotNull(actual.Token);
        Assert.True(actual.Validity > 0);
    }

    [Fact]
    public void Test_UserExists_PostAuthGoogleWithGoodToken_ServerReturnsToken() {
        HttpContext context = ApiTestUtils.FakeContext(TestEmail, $"{{\"jwttoken\": \"{TestGoogleToken}\"}}");

        Auth auth = CreateAuth();
        Users users = UserTestsUtils.CreatePopulated();

        AuthApi authApi = new AuthApi(auth, users);

        var result = authApi.PostAuthGoogle(context.Request).Result;

        Assert.IsType<Ok<AuthApi.GoogleAuthResponse>>(result);

        var response = (Ok<AuthApi.GoogleAuthResponse>)result;

        var actual = (AuthApi.GoogleAuthResponse?)response.Value;

        Assert.NotNull(actual);
        Assert.NotNull(actual.Token);
        Assert.True(actual.Validity > 0);
    }

    [Fact]
    public void Test_UserNameChanges_PostAuthGoogleWithGoodToken_UsersNameChanges() {
        var NEW_NAME = "Shell Heinert";

        HttpContext context = ApiTestUtils.FakeContext(TestEmail, $"{{\"jwttoken\": \"{TestGoogleToken}\"}}");

        Auth auth = CreateAuth(TestEmail, NEW_NAME);
        Users users = UserTestsUtils.CreatePopulated();

        AuthApi authApi = new AuthApi(auth, users);

        var result = authApi.PostAuthGoogle(context.Request).Result;

        User? user = users.FindUser(TestEmail);

        Assert.IsType<Ok<AuthApi.GoogleAuthResponse>>(result);

        var response = (Ok<AuthApi.GoogleAuthResponse>)result;

        var actual = (AuthApi.GoogleAuthResponse?)response.Value;

        Assert.NotNull(actual);
        Assert.NotNull(actual.Token);
        Assert.True(actual.Validity > 0);

        Assert.NotNull(user);
        Assert.Equal(NEW_NAME, user.Name);
    }

    [Fact]
    public void Test_PostAuthGoogleWithInvalidJson_ServerReturnsBadRequest() {
        HttpContext context = ApiTestUtils.FakeContext(TestEmail, $"{{\"jwttoken:");

        Auth auth = CreateAuth();
        Users users = UserTestsUtils.CreateEmpty();

        AuthApi authApi = new AuthApi(auth, users);

        var result = authApi.PostAuthGoogle(context.Request).Result;

        Assert.IsType<BadRequest<ApiBase.ErrorResponse>>(result);
    }

    [Fact]
    public void Test_PostAuthGoogleWithInvalidToken_ServerReturnsBadRequest() {
        HttpContext context = ApiTestUtils.FakeContext(TestEmail, $"{{\"jwttoken\": \"{TestGoogleTokenBad}\"}}");

        Auth auth = CreateAuth();
        Users users = UserTestsUtils.CreateEmpty();

        AuthApi authApi = new AuthApi(auth, users);

        var result = authApi.PostAuthGoogle(context.Request).Result;

        Assert.IsType<BadRequest<ApiBase.ErrorResponse>>(result);
    }
}
