using System;
using System.Net.Http;
using System.Threading.Tasks;

public class UserService
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl = "https://accountserviceprovider-g5gnanhufngbezgt.swedencentral-01.azurewebsites.net";

    public UserService()
    {
        _httpClient = new HttpClient();
    }

    public async Task<bool> CheckUserExist(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("User ID cannot be null or empty.", nameof(userId));

        var requestUrl = $"{_baseUrl}/api/UserExist/userexist?id={Uri.EscapeDataString(userId)}";

        var request = new HttpRequestMessage(HttpMethod.Post, requestUrl)
        {
            Content = new StringContent(string.Empty)
        };
        request.Headers.Accept.ParseAdd("*/*");

        try
        {
            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new ApplicationException($"User check failed: {errorContent}");
            }

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error checking user existence: {ex.Message}");
            throw;
        }
    }
}
