using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    private IConfiguration? Configuration;

    private static readonly string GoogleApiClientId = "A";
    private static readonly string JwtIssuer = "localhost";
    private static readonly string JwtSecretKey = "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA";

    private static readonly Dictionary<string, string?> TestConfiguration = new Dictionary<string, string?>
    {
        {"GoogleApiClientId", GoogleApiClientId},
        {"JwtSecretKey", JwtSecretKey},
        {"JwtIssuer", JwtIssuer},
        {"DatabaseConnectionString", "Data Source=:memory:"}
    };

    public Auth GetAuth()
    {
        return new Auth(JwtSecretKey, JwtIssuer, new GoogleTokenValidator(GoogleApiClientId));
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
