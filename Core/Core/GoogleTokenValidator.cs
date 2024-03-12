using Google.Apis.Auth;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.JsonWebTokens;

public class GoogleTokenValidator : JsonWebTokenHandler
{
    private GoogleJsonWebSignature.ValidationSettings Settings;

    public GoogleTokenValidator(string audience)
    {
        Settings = new GoogleJsonWebSignature.ValidationSettings() { Audience = new string[] { audience } };
    }
    public override async Task<TokenValidationResult> ValidateTokenAsync(String token, TokenValidationParameters parameters)
    {
        try
        {
            var payload = await GoogleJsonWebSignature.ValidateAsync(token, Settings);

            SecurityToken securityToken = ReadToken(token);

            var claims = new List<Claim> {
                new Claim(ClaimTypes.Role, "User"),
                new Claim(ClaimTypes.NameIdentifier, payload.Name),
                new Claim(ClaimTypes.Name, payload.Name),
                new Claim(ClaimTypes.Email, payload.Email),
                new Claim(ClaimTypes.GivenName, payload.GivenName),
                new Claim(ClaimTypes.Surname, payload.FamilyName)
            };

            return new TokenValidationResult() { Issuer = payload.Issuer, ClaimsIdentity = new ClaimsIdentity(claims), SecurityToken = securityToken, IsValid = true };
        }
        catch (InvalidJwtException)
        {
            return new TokenValidationResult { IsValid = false };
        }
    }
}
