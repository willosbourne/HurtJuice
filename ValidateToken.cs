using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace HurtJuice;

public class ValidateToken
{
    private readonly ILogger _logger;

    public class RequestBody
    {
        public string url { get; set; }
    }


    public ValidateToken(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<ValidateToken>();
    }

    [Function("ValidateToken")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req,
        FunctionContext executionContext)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");

        // Read the request body and deserialize it into the RequestBody class
        string requestBodyString = await new StreamReader(req.Body).ReadToEndAsync();
        RequestBody? requestBody = JsonSerializer.Deserialize<RequestBody>(requestBodyString);

        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
        

        var url = requestBody?.url ?? string.Empty;

        //helpjuice.com/jwt/sht-connect?jwt=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJlbWFpbCI6IndpbGxvc2JvdXJuZUBzZXZlbmhpbGxzdGVjaG5vbG9neS5jb20iLCJqdGkiOiJhODdmMDVhZi1mZTZjLTQwZjYtYWJjMy1mYzVlNTczZThlNDAiLCJuYmYiOjE3MDk5MDQzNTksImV4cCI6MTcxMDUwOTE1OSwiaWF0IjoxNzA5OTA0MzU5fQ.yY5g119CxEwvECSjgF-EBJ9Y1W_n-H6HVLsJ36nmLUM&fallback_url=https://sht-connect.helpjuice.com

        if (StringReplacer.HasReplacementInString(url) || string.IsNullOrEmpty(url) || !url.Contains("helpjuice.com"))
        {
            _logger.LogInformation("----------------Invalid URL----------------");
            await response.WriteStringAsync("false");
        }
        else
        {
            _logger.LogInformation("+++++++++++++++++Valid URL+++++++++++++++++");
            await response.WriteStringAsync("true");
        }
        return response;
        
    }
}