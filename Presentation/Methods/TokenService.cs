using Presentation.Models;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using System.Net.Http.Json;
using Presentation;
using System.Net.Http.Headers;


namespace Presentation.Methods;

public class TokenService : ITokenService
{
    public async Task<TokenResponse> GenerateAccessToken(TokenRequest tokenRequest, int expiresInDays = 30)
    {


        try
        {

            if (string.IsNullOrEmpty(tokenRequest.UserId))
            {
                throw new NullReferenceException("No userId provided");
            }


            var issuer = Environment.GetEnvironmentVariable("Issuer") ?? throw new NullReferenceException("No issuer provided");
            var audience = Environment.GetEnvironmentVariable("Audience") ?? throw new NullReferenceException("No audience provided");
            var secretKey = Environment.GetEnvironmentVariable("SecretKey") ?? throw new NullReferenceException("No secret key provided");
            var credentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)), SecurityAlgorithms.HmacSha256) ?? throw new NullReferenceException("Unable to create credentials");

            List<Claim> claims = [new Claim(ClaimTypes.NameIdentifier, tokenRequest.UserId)];




            var userService = new UserService();
            bool exists = await userService.CheckUserExist(tokenRequest.UserId);




            if (!string.IsNullOrEmpty(tokenRequest.Email))
            {
                claims.Add(new Claim(ClaimTypes.Email, tokenRequest.Email));
            }

            if (!string.IsNullOrEmpty(tokenRequest.Role))
            {
                claims.Add(new Claim(ClaimTypes.Role, tokenRequest.Role));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = credentials,
                Expires = DateTime.UtcNow.AddDays(expiresInDays)

            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return new TokenResponse
            {
                Succeeded = true,
                AccessToken = tokenHandler.WriteToken(token),
                Message = "Token generated successfully"
            };
        }
        catch (Exception ex)
        {
            return new TokenResponse
            {
                Succeeded = false,
                Message = ex.Message
            };
        }
    }

    public async Task<ValidationResponse> ValidateAccessToken(ValidationRequest validationRequest)
    {
        var issuer = Environment.GetEnvironmentVariable("Issuer") ?? throw new NullReferenceException("No issuer provided");
        var audience = Environment.GetEnvironmentVariable("Audience") ?? throw new NullReferenceException("No audience provided");
        var secretKey = Environment.GetEnvironmentVariable("SecretKey") ?? throw new NullReferenceException("No secret key provided");

        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            var principal = tokenHandler.ValidateToken(validationRequest.AccessToken, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = issuer,
                ValidateAudience = true,
                ValidAudience = audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                ClockSkew = TimeSpan.Zero,

            }, out SecurityToken validatedToken);

            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new NullReferenceException("UserId in claims is null");


            var userService = new UserService();
            bool exists = await userService.CheckUserExist(validationRequest.UserId);


            return new ValidationResponse
            {
                Succeeded = true,
            };

        }
        catch (Exception ex)
        {
            return new ValidationResponse
            {
                Succeeded = false,
                Message = ex.Message
            };
        }
    }
}



