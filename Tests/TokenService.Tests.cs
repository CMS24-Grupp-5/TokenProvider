using Moq;
using Presentation.Methods;
using Presentation.Models;
using Xunit;
using System;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using System.Collections.Generic;
using System.Security.Claims;

namespace Tests
{
    public class TokenServiceTests
    {
        private readonly TokenService _tokenService;

        public TokenServiceTests()
        {
            _tokenService = new TokenService();

            Environment.SetEnvironmentVariable("Issuer", "TestIssuer");
            Environment.SetEnvironmentVariable("Audience", "TestAudience");
            Environment.SetEnvironmentVariable("SecretKey", "6eefaf37-0f12-45d7-be83-a9c90079c638");
        }

        [Fact]
        public async Task GenerateAccessToken_ValidRequest_ReturnsSuccess()
        {
            var request = new TokenRequest
            {
                UserId = "user123",
                Email = "user@example.com",
                Role = "admin"
            };

            var result = await _tokenService.GenerateAccessToken(request);

            Assert.True(result.Succeeded);
            Assert.NotNull(result.AccessToken);
            Assert.Equal("Token generated successfully", result.Message);
        }

        [Fact]
        public async Task GenerateAccessToken_MissingUserId_ReturnsFailure()
        {
            var request = new TokenRequest
            {
                Email = "user@example.com",
                Role = "admin"
            };

            var result = await _tokenService.GenerateAccessToken(request);

            Assert.False(result.Succeeded);
            Assert.Equal("No userId provided", result.Message);
        }

        [Fact]
        public async Task GenerateAccessToken_MissingEnvironmentVariables_ReturnsFailure()
        {
            Environment.SetEnvironmentVariable("Issuer", null);

            var request = new TokenRequest
            {
                UserId = "user123"
            };

            var result = await _tokenService.GenerateAccessToken(request);

            Assert.False(result.Succeeded);
            Assert.Equal("No issuer provided", result.Message);
        }

        [Fact]
        public async Task ValidateAccessToken_ValidToken_ReturnsSuccess()
        {
            var request = new TokenRequest
            {
                UserId = "user123"
            };

            var generateResult = await _tokenService.GenerateAccessToken(request);

            Assert.True(generateResult.Succeeded);

            var validationRequest = new ValidationRequest
            {
                AccessToken = generateResult.AccessToken
            };

            var validateResult = await _tokenService.ValidateAccessToken(validationRequest);

            Assert.True(validateResult.Succeeded);
            Assert.Null(validateResult.Message);
        }

        [Fact]
        public async Task ValidateAccessToken_InvalidToken_ReturnsFailure()
        {
            var validationRequest = new ValidationRequest
            {
                AccessToken = "invalid_token_string"
            };

            var result = await _tokenService.ValidateAccessToken(validationRequest);

            Assert.False(result.Succeeded);
            Assert.Contains("IDX", result.Message);
        }
    }
}
