using System.Net.Http.Headers;
using System.Net;

public class AuthorizationTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;

    public AuthorizationTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    private void SetJwtToken(HttpClient client)
    {
        var auth = _factory.GetAuth();
        var token = auth.GenerateJWT(AuthApiTests.TestEmail, new TimeSpan(1, 0, 0, 0));

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    private static readonly string[] GetEndpointsAuth =
    {
        "/user/profile",
        "/bookplan/list",
        "/bookplan/get/1"
    };

    private static readonly string[] PostEndpointsAuth =
    {
        "/bookplan/add",
        "/bookplan/remove",
        "/bookplan/edit"
    };

    public static IEnumerable<object[]> GetTestGetEndpointsAuth() {
        foreach (var endpoint in GetEndpointsAuth)
        {
            yield return new object[] {endpoint};
        }
    }

    public static IEnumerable<object[]> GetTestPostEndpointsAuth() {
        foreach (var endpoint in PostEndpointsAuth)
        {
            yield return new object[] {endpoint};
        }
    }

    [Theory]
    [MemberData(nameof(GetTestGetEndpointsAuth))]
    public async Task Test_EndpointRequiresAuth_UnauthorizedGetRequest_ReturnsUnauthorized(string endpoint) {
        var client = _factory.CreateClient();

        var response = await client.GetAsync(endpoint);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Theory]
    [MemberData(nameof(GetTestGetEndpointsAuth))]
    public async Task Test_EndpointRequiresAuth_AuthorizedGetRequest_DoesNotReturnUnauthorized(string endpoint) {
        var client = _factory.CreateClient();

        SetJwtToken(client);

        var response = await client.GetAsync(endpoint);

        Assert.NotEqual(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Theory]
    [MemberData(nameof(GetTestPostEndpointsAuth))]
    public async Task Test_EndpointRequiresAuth_UnauthorizedPostRequest_ReturnsUnauthorized(string endpoint) {
        var client = _factory.CreateClient();

        var response = await client.PostAsync(endpoint, null);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Theory]
    [MemberData(nameof(GetTestGetEndpointsAuth))]
    public async Task Test_EndpointRequiresAuth_AuthorizedPostRequest_DoesNotReturnUnauthorized(string endpoint) {
        var client = _factory.CreateClient();

        SetJwtToken(client);

        var response = await client.PostAsync(endpoint, null);

        Assert.NotEqual(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
