using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

public class Auth
{
    private JsonWebTokenHandler Handler;
    private SigningCredentials Credentials;
    private SymmetricSecurityKey Key;
    private string Issuer;
    private GoogleTokenValidator GoogleTokenValidator;

    public Auth(string key, string issuer, GoogleTokenValidator validator)
    {
        Handler = new JsonWebTokenHandler();
        Key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        Credentials = new SigningCredentials(Key, SecurityAlgorithms.HmacSha256);
        Issuer = issuer;
        GoogleTokenValidator = validator;
    }

    public static string GetNameIdentifier(HttpContext context)
    {
        var email = context.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

        if (email == null)
            throw new KeyNotFoundException("NameIdentifier claim not found");

        return email.Value;
    }

    public static string GenerateSecret(int length = 16)
    {
        var alphanum = "ABCDEFGHIJKLMONOPQRSTUVWXYZabcdefghijklmonopqrstuvwxyz0123456789";

        char[] chars = new char[length];

        for (int i = 0; i < length; i++)
        {
            chars[i] = alphanum[RandomNumberGenerator.GetInt32(0, alphanum.Length)];
        }

        return new string(chars);
    }

    public static string GenerateHash(User user, string data)
    {
        string payload = $"{user.Secret}{user.SecretVersion}{data}";
        byte[] bytes = Encoding.ASCII.GetBytes(payload);

        var hash = SHA256.Create().ComputeHash(bytes);

        return Convert.ToHexString(hash).ToLower();
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
        if (subject.Length == 0)
            throw new ArgumentException("Subject can't be empty");

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
            var payload = GoogleTokenValidator.ValidateToken(token);

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
