using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System.Text;
using Newtonsoft.Json;

public class ApiTestUtils
{
    public static HttpContext FakeContext(string? email = null, string? data = null)
    {
        HttpContext context = new DefaultHttpContext();

        ClaimsIdentity identity = new ClaimsIdentity();

        if (email is not null)
        {
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, email));
        }

        context.User = new ClaimsPrincipal(identity);

        if (data is not null)
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(data));
            var writer = new StreamWriter(stream);

            context.Request.Body = stream;
            context.Request.ContentType = "application/json";
        }

        return context;
    }
}