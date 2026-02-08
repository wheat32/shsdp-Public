using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;

namespace SHSDP.Code;

public class ApiArbitrator
{
	private readonly HttpClient _client;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public ApiArbitrator(HttpClient client)
    {
        _client = client;
    }

    // ------------ GET with Query Properties ------------ //
    public async Task<T> GetFromApiAsync<T>(String url, String apiToken, Object requestProperties)
    {
        Dictionary<String, String?> queryDict = [];

        foreach (PropertyInfo prop in requestProperties.GetType().GetProperties())
        {
            Object? value = prop.GetValue(requestProperties);
            if (value != null)
            {
                queryDict[prop.Name] = value.ToString();
            }
        }

        String finalUrl = QueryHelpers.AddQueryString(url, queryDict);
        return await GetFromApiAsync<T>(finalUrl, apiToken);
    }

    // ------------ GET ------------ //
    public async Task<T> GetFromApiAsync<T>(String url, String apiToken)
    {
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", apiToken);
        
        HttpResponseMessage response = await _client.GetAsync(url);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<T>(JsonOptions)
               ?? throw new InvalidOperationException("API returned null JSON.");
    }

    // ------------ POST ------------ //
    public async Task<T> PostToApiAsync<T>(String url, String apiToken, Object data)
    {
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", apiToken);

        StringContent content = new StringContent(
            JsonSerializer.Serialize(data, JsonOptions),
            Encoding.UTF8,
            "application/json");

        HttpResponseMessage response = await _client.PostAsync(url, content);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<T>(JsonOptions)
               ?? throw new InvalidOperationException("API returned null JSON.");
    }

    // ------------ PUT ------------ //
    public async Task<T> PutToApiAsync<T>(String url, String apiToken, Object data)
    {
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", apiToken);

        StringContent content = new StringContent(
            JsonSerializer.Serialize(data, JsonOptions),
            Encoding.UTF8,
            "application/json");

        HttpResponseMessage response = await _client.PutAsync(url, content);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<T>(JsonOptions)
               ?? throw new InvalidOperationException("API returned null JSON.");
    }

    // ------------ DELETE with Query Props ------------ //
    public async Task<T> DeleteFromApiAsync<T>(String url, String apiToken, Object requestProperties)
    {
        var queryDict = new Dictionary<String, String?>();

        foreach (PropertyInfo prop in requestProperties.GetType().GetProperties())
        {
            Object? value = prop.GetValue(requestProperties);
            if (value != null)
            {
                queryDict[prop.Name] = value.ToString();
            }
        }

        String finalUrl = QueryHelpers.AddQueryString(url, queryDict);
        return await DeleteFromApiAsync<T>(finalUrl, apiToken);
    }

    // ------------ DELETE ------------ //
    public async Task<T> DeleteFromApiAsync<T>(String url, String apiToken)
    {
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", apiToken);

        HttpResponseMessage response = await _client.DeleteAsync(url);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<T>(JsonOptions)
               ?? throw new InvalidOperationException("API returned null JSON.");
    }
}