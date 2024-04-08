using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    private IConfiguration? Configuration;

    private static readonly Dictionary<string, string?> TestConfiguration = new Dictionary<string, string?>
    {
        {"GoogleApiClientId", ""},
        {"JwtIssuer", "localhost"},
        {"JwtSecretKey", "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA"},
        {"DatabaseConnectionString", "Data Source=:memory:"}
    };

    public Auth GetAuth()
    {
        return new Auth(TestConfiguration["JwtSecretKey"], TestConfiguration["JwtIssuer"], new GoogleTokenValidator(TestConfiguration["GoogleApiClientId"]));
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(TestConfiguration)
            .Build();

        builder.ConfigureAppConfiguration(builder =>
        {
            builder.Sources.Clear();
            builder.AddConfiguration(configuration);
        });
    }
}
