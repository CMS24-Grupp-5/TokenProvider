using Presentation.Models;

namespace Presentation.Methods;

public interface ITokenService
{
    Task<TokenResponse> GenerateAccessToken(TokenRequest tokenRequest, int expiresInDays = 30);
    Task<ValidationResponse> ValidateAccessToken(ValidationRequest validationRequest);
}