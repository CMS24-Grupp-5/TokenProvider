using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Presentation.Methods;
using Presentation.Models;

namespace Presentation.Functions;

public class ValidateToken(ILogger<ValidateToken> logger, ITokenService tokenService)
{
    private readonly ILogger<ValidateToken> _logger = logger;
    private readonly ITokenService _tokenService = tokenService;

    public static BadRequestObjectResult BadRequest(string message)
    {
        return new BadRequestObjectResult(new TokenResponse
        {
            Succeeded = false,
            Message = message
        });
    }

    [Function("ValidateToken")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
    {
        try
        {
            var body = await new StreamReader(req.Body).ReadToEndAsync();

            if (string.IsNullOrEmpty(body))
            {
                return BadRequest("Request body is null");
            }

            ValidationRequest? tokenRequest;
            try
            {
                tokenRequest = JsonConvert.DeserializeObject<ValidationRequest>(body);
                if (tokenRequest == null)
                {
                    throw new JsonException("Deserialize returned null");
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation("C# HTTP trigger function processed a request.");
                return BadRequest("Unable to deserialize body");
            }

            var response = await _tokenService.ValidateAccessToken(tokenRequest);
            return response.Succeeded
                ? new OkObjectResult(response)
                : BadRequest(response.Message!);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled error in validate token");
            return BadRequest("An unexpected error occurred");
        }
    }
}
