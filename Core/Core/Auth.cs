using Google.Apis.Auth;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json;
using System.Text;

public class Auth {
    private JsonWebTokenHandler Handler;
    private SigningCredentials Credentials;
    private SymmetricSecurityKey Key;
    private string Issuer;
    private GoogleJsonWebSignature.ValidationSettings GoogleValidationSettings;

    public Auth(string key, string issuer, string googleClientId) {
        Handler = new JsonWebTokenHandler();
        Key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        Credentials = new SigningCredentials(Key, SecurityAlgorithms.HmacSha256);
        Issuer = issuer;
        GoogleValidationSettings = new GoogleJsonWebSignature.ValidationSettings() {Audience = new string[] {googleClientId}};
    }

    public String GetIssuer() {
        return Issuer;
    }

    public String GetAudience() {
        return Issuer;
    }

    public SymmetricSecurityKey GetSigningKey() {
        return Key;
    }

    private string GenerateJWT(string subject, TimeSpan expires) {
        var claims = new Dictionary<string, object>{
            {JwtRegisteredClaimNames.Sub, subject}
        };

        var tokenDescriptor = new SecurityTokenDescriptor() {
            Issuer = Issuer,
            Audience = Issuer,
            IssuedAt = DateTime.Now,
            NotBefore = DateTime.Now,
            Expires = DateTime.Now.Add(expires),
            SigningCredentials = Credentials,
            Claims = claims
        };

        return Handler.CreateToken(tokenDescriptor);
    }

    private bool ValidateGoogleJWT(string token, ref string email) {
        try {
            var payload = GoogleJsonWebSignature.ValidateAsync(token, GoogleValidationSettings).Result;

            if (!payload.EmailVerified)
                return false;

            email = payload.Email;

            return true;
        }
        catch (Exception) {
            return false;
        }
    }

    private class GoogleAuthRequest {
        public string JwtToken { get; set; } = null!;
    };

    public void Map(WebApplication app) {
        app.MapPost("/auth/google", async (HttpRequest request) => {
            var stream = await new StreamReader(request.Body).ReadToEndAsync();
            var req = JsonSerializer.Deserialize<GoogleAuthRequest>(stream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (req == null || req.JwtToken == null)
                return Results.BadRequest(new {Error = "jwttoken is not specified."});

            string email = string.Empty;

            if (!ValidateGoogleJWT(req.JwtToken, ref email))
                return Results.BadRequest(new {Error = "jwttoken is invalid."});

            string jwt = GenerateJWT(email, new TimeSpan(1, 0, 0, 0));

            return Results.Ok(new {Token = jwt});
        });
    }
}
