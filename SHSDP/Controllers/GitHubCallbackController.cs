using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;

namespace SHSDP.Controllers;

[Route("github-callback")]
public class GitHubCallbackController : Controller
{
    private const String GitHubTokenEndpoint = "https://github.com/login/oauth/access_token";
    private const String GitHubUserApi = "https://api.github.com/user";

#if DEBUG
    private const String RedirectAfterLogin = "https://localhost:7200/GitHubReinvite";
#else
    private const String RedirectAfterLogin = "https://shsdp.dev/GitHubReinvite";
#endif

    private readonly IHttpClientFactory _httpClientFactory;

    public GitHubCallbackController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [HttpGet]
    public async Task<IActionResult> Callback([FromQuery] String code, [FromQuery] String error)
    {
        if (String.IsNullOrWhiteSpace(error) == false)
        {
            return Redirect($"{RedirectAfterLogin}?error={Uri.EscapeDataString(error)}");
        }

        if (String.IsNullOrWhiteSpace(code))
        {
            return Redirect($"{RedirectAfterLogin}?error=missing_code");
        }

        String? accessToken = await ExchangeCodeForTokenAsync(code);
        if (String.IsNullOrWhiteSpace(accessToken))
        {
            return Redirect($"{RedirectAfterLogin}?error=token_exchange_failed");
        }

        String? login = await GetGitHubLoginAsync(accessToken);
        if (String.IsNullOrWhiteSpace(login))
        {
            return Redirect($"{RedirectAfterLogin}?error=user_fetch_failed");
        }

        // Redirect back to the main page with the GitHub login
        return Redirect($"{RedirectAfterLogin}?login={Uri.EscapeDataString(login)}");
    }

    private async Task<String?> ExchangeCodeForTokenAsync(String code)
    {
        HttpClient client = _httpClientFactory.CreateClient();

        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, GitHubTokenEndpoint)
        {
            Content = new FormUrlEncodedContent(new Dictionary<String, String>
            {
                { "client_id", Program.GITHUB_CLIENT_ID },
                { "client_secret", Program.GITHUB_CLIENT_SECRET },
                { "code", code }
            })
        };

        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        HttpResponseMessage response = await client.SendAsync(request);
        String json = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode == false)
        {
            return null;
        }

        using JsonDocument doc = JsonDocument.Parse(json);
        if (doc.RootElement.TryGetProperty("access_token", out JsonElement tokenEl))
        {
            return tokenEl.GetString();
        }

        return null;
    }

    private async Task<String?> GetGitHubLoginAsync(String token)
    {
        HttpClient client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        client.DefaultRequestHeaders.UserAgent.ParseAdd("GitHubReinviteTool");

        HttpResponseMessage response = await client.GetAsync(GitHubUserApi);
        if (response.IsSuccessStatusCode == false)
        {
            return null;
        }
        
        String json = await response.Content.ReadAsStringAsync();
        using JsonDocument doc = JsonDocument.Parse(json);

        if (doc.RootElement.TryGetProperty("login", out JsonElement loginEl))
        {
            return loginEl.GetString();
        }

        return null;
    }
}
