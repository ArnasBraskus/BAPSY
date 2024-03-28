using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Text;

public class AuthTests {
    public static string TestSecretKey = "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA";
    public static string TestIssuer = "localhost";
    public static string TestGoogleClientId = String.Empty;

    private Auth CreateAuth() {
        return new Auth(TestSecretKey, TestIssuer, new GoogleTokenValidator(TestGoogleClientId));
    }

    public TokenValidationResult ValidateToken(string token) {
        var parameters = new TokenValidationParameters() {
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidateAudience = true,
            ValidIssuer = TestIssuer,
            ValidAudience = TestIssuer,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TestSecretKey))
        };

        return new JsonWebTokenHandler().ValidateTokenAsync(token, parameters).Result;
    }

    [Fact]
    public void Test_GenerateJWT_TokenIsValid() {
        var SUBJECT = "user1";
        var EXPIRATION = new TimeSpan(1, 0, 0, 0);

        Auth auth = CreateAuth();

        string token = auth.GenerateJWT(SUBJECT, EXPIRATION);

        var result = ValidateToken(token);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Test_GenerateJWTWithEmptySubject_ThrowsException() {
        var SUBJECT = String.Empty;
        var EXPIRATION = new TimeSpan(1, 0, 0, 0);

        Auth auth = CreateAuth();

        Assert.Throws<ArgumentException>(() => auth.GenerateJWT(SUBJECT, EXPIRATION));
    }

    [Theory]
    [MemberData(nameof(UserTestsUtils.GetTestUsers1Emails), MemberType = typeof(UserTestsUtils))]
    public void Test_GenerateJWT_TokenSubjectMatches(string subject) {
        var EXPIRATION = new TimeSpan(1, 0, 0, 0);

        Auth auth = CreateAuth();

        string token = auth.GenerateJWT(subject, EXPIRATION);

        var result = ValidateToken(token);

        Assert.Equal(subject, result.Claims[JwtRegisteredClaimNames.Sub]);
    }

    [Fact]
    public void Test_ValidateGoogleJwtUsingEmptyToken_ShouldFail() {
        Auth auth = CreateAuth();

        string email = String.Empty;
        string name = String.Empty;

        var actual = auth.ValidateGoogleJWT(String.Empty, ref email, ref name);

        Assert.False(actual);
    }

    [Theory]
    [InlineData("AAAAAA")]
    [InlineData("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJhdWQiOiJsb2NhbGhvc3QiLCJpc3MiOiJsb2NhbGhvc3QiLCJleHAiOjE3MTE2MjYzNTgsImlhdCI6MTcxMTUzOTk1OCwibmJmIjoxNzExNTM5OTU4LCJzdWIiOiJ1c2VyMSJ9.3zvqYKs7fUiCowyBYyeA9AyLxJd3XvK7oJorO65oN3M")]
    [InlineData("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJhdWQiOiJsb2NhbGhvc3QiLCJpc3MiOiJsb2NhbGhvc3QiLCJleHAiOjE3MTE2MjY4NzcsImlhdCI6MTcxMTU0MDQ3NywibmJmIjoxNzExNTQwNDc3LCJzdWIiOiJiZHVja2VyMEBlaG93LmNvbSJ9.VCpHqU4wihdebTBMVtfC6VdExKrjkmuUYiMzIFAGLps")]
    public void Test_ValidateGoogleJwtUsingInvalidToken_ShouldFail(string token) {
        Auth auth = CreateAuth();

        string email = String.Empty;
        string name = String.Empty;

        var actual = auth.ValidateGoogleJWT(token, ref email, ref name);

        Assert.False(actual);
    }

}
