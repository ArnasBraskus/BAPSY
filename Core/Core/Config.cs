public class Config
{
    public string DatabaseConnectionString { get; set; } = null!;
    public string JwtSecretKey { get; set; } = null!;
    public string JwtIssuer { get; set; } = null!;
    public string GoogleApiClientId { get; set; } = null!;
}
