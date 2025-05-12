using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Presentation.Methods;
using Presentation.Models;

namespace Presentation.Functions;

public class GenerateToken(ILogger<GenerateToken> logger, ITokenService tokenService)
{
    private readonly ILogger<GenerateToken> _logger = logger;
    private readonly ITokenService _tokenService = tokenService;

    [Function("GenerateToken")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
    {
        var body = await new StreamReader(req.Body).ReadToEndAsync();
        if (string.IsNullOrEmpty(body))
        {
            _logger.LogInformation("Invalid body");

            return new BadRequestObjectResult
                (new TokenResponse
                {
                    Succeeded = false,
                    Message = "Request body is null"
                });
        }

        try
        {
            var tokenRequest = JsonConvert.DeserializeObject<TokenRequest>(body) ?? throw new NullReferenceException("Unable to deserialize body");

            var tokenResponse = await _tokenService.GenerateAccessToken(tokenRequest);
            if (tokenResponse.Succeeded)
            {
                return new OkObjectResult(tokenResponse);
            }
            else
            {
                return new BadRequestObjectResult(tokenResponse);
            }
        }
        catch (Exception ex)
        {
            _logger.LogInformation(ex.Message);

            return new BadRequestObjectResult
                   (new TokenResponse
                   {
                       Succeeded = false,
                       Message = ex.Message
                   });
        }
    }
}
