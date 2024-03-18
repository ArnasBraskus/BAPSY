using Google.Apis.Auth;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;
using System.Text;

public class Auth
{
    private Users Users;
    private JsonWebTokenHandler Handler;
    private SigningCredentials Credentials;
    private SymmetricSecurityKey Key;
    private string Issuer;
    private GoogleJsonWebSignature.ValidationSettings GoogleValidationSettings;

    public Auth(Users users, string key, string issuer, string googleClientId)
    {
        Handler = new JsonWebTokenHandler();
        Users = users;
        Key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        Credentials = new SigningCredentials(Key, SecurityAlgorithms.HmacSha256);
        Issuer = issuer;
        GoogleValidationSettings = new GoogleJsonWebSignature.ValidationSettings() { Audience = new string[] { googleClientId } };
    }

    public static string? GetNameIdentifier(HttpContext context) {
        var email = context.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

        if (email == null)
            return null;

        return email.Value;
    }

    public String GetIssuer()
    {
        return Issuer;
    }

    public String GetAudience()
    {
        return Issuer;
    }

    public SymmetricSecurityKey GetSigningKey()
    {
        return Key;
    }

    public void Add(IServiceCollection services)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidIssuer = Issuer,
                ValidAudience = Issuer,
                IssuerSigningKey = Key,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true
            };
        });

    }

    public string GenerateJWT(string subject, TimeSpan expires)
    {
        var claims = new Dictionary<string, object>{
            {JwtRegisteredClaimNames.Sub, subject}
        };

        var tokenDescriptor = new SecurityTokenDescriptor()
        {
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

    public bool ValidateGoogleJWT(string token, ref string email, ref string name)
    {
        try
        {
            var payload = GoogleJsonWebSignature.ValidateAsync(token, GoogleValidationSettings).Result;

            if (!payload.EmailVerified)
                return false;

            email = payload.Email;
            name = payload.Name;

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
