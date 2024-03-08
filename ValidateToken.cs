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

        if (StringReplacer.HasReplacementInString(url) || string.IsNullOrEmpty(url))
        {
            await response.WriteStringAsync("false");
        }
        else
        {
            await response.WriteStringAsync("true");
        }
        return response;
        
    }
}