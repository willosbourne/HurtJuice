using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace HurtJuice;

public class GetToken
{
    private readonly ILogger _logger;
    private readonly string _jwtUrl;
    private readonly string _jwtSecret;
    private static readonly Random Random = new Random();

    public GetToken(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<GetToken>();
        _jwtUrl = "https://helpjuice.com/jwt/sht-connect";
        _jwtSecret = Environment.GetEnvironmentVariable("HELPJUICE_API_SECRET") ?? string.Empty;
    }
    
    public class RequestBody
    {
        public string email { get; set; }
    }

    [Function("GetToken")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req,
        FunctionContext executionContext)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");
        if (Random.NextDouble() < 0.3)
        {
            var badResponse = req.CreateResponse((HttpStatusCode) 418);
            badResponse.Headers.Add("Content-Type", "text/plain; charset=utf-8");
            return badResponse;
        }
        
        string requestBodyString = await new StreamReader(req.Body).ReadToEndAsync();
        RequestBody? requestBody = JsonSerializer.Deserialize<RequestBody>(requestBodyString);

        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

        var redirectUrl = "didn't get url";
        if (Random.NextDouble() < 0.2)
        {
            redirectUrl = "service unavailable";
        }
        else if (!string.IsNullOrEmpty(requestBody.email))
        {
            redirectUrl = GetHelpjuiceRedirectUrl(requestBody.email);
            redirectUrl = StringReplacer.ReplaceMiddleRandomly(redirectUrl, _logger);
        }
        else
        {
            redirectUrl = "bad email";
        }

        response.WriteString(redirectUrl);

        return response;
    }
    
    private string GetHelpjuiceRedirectUrl(string email, string fallbackUrl = "")
    {
        if (string.IsNullOrEmpty(fallbackUrl))
        {
            fallbackUrl = $"https://sht-connect.helpjuice.com";
        }
            
        String jwt = GenerateHelpjuiceJwtToken(email);
        String url = $"{_jwtUrl}?jwt={jwt}";
            
        if (!string.IsNullOrEmpty(fallbackUrl))
        {
            url = $"{url}&fallback_url={fallbackUrl}";
        }
            
        return url;
    }
    
    public String GenerateHelpjuiceJwtToken(String email)
    {
        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
        Byte[] key = Encoding.ASCII.GetBytes(_jwtSecret);
        SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] {
                new Claim("email", email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}