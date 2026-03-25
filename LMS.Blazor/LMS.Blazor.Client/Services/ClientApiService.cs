using Microsoft.AspNetCore.Components;
using System.Net;
using System.Text;
using System.Text.Json;

namespace LMS.Blazor.Client.Services;

public class ClientApiService : IApiService
{
    private readonly HttpClient _httpClient;
    private readonly NavigationManager _navigationManager;
    private readonly JsonSerializerOptions _jsonOptions;

    public ClientApiService(HttpClient httpClient, NavigationManager navigationManager)
    {
        _httpClient = httpClient;

        _navigationManager = navigationManager;

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<T?> GetAsync<T>(string endpoint, CancellationToken ct = default)
    {
        using var response = await _httpClient.GetAsync($"api/proxy/{endpoint}", ct);
        return await ReadResponseAsync<T>(response, ct);
    }

    public async Task<TResult?> PatchAsync<TParam, TResult>(string endpoint, TParam body, CancellationToken token = default)
    {
        using var jsonContent = Serialize(body);
        using var response = await _httpClient.PatchAsync($"api/proxy/{endpoint}", jsonContent, token);
        return await ReadResponseAsync<TResult>(response, token);
    }

    public async Task<TResult?> PostAsync<TParam, TResult>(string endpoint, TParam body, CancellationToken token = default)
    {
        using var jsonContent = Serialize(body);
        using var response = await _httpClient.PostAsync($"api/proxy/{endpoint}", jsonContent, token);
        return await ReadResponseAsync<TResult>(response, token);
    }

    public async Task PutAsync<TParam>(string endpoint, TParam body, CancellationToken token = default)
    {
        using var jsonContent = Serialize(body);
        using var response = await _httpClient.PutAsync($"api/proxy/{endpoint}", jsonContent, token);
        await EnsureSuccessAsync(response, token);
    }

    public async Task DeleteAsync(string endpoint, CancellationToken token = default)
    {
        using var response = await _httpClient.DeleteAsync($"api/proxy/{endpoint}", token);
        await EnsureSuccessAsync(response, token);
    }

    private StringContent Serialize<T>(T obj)
    {
        return new(
            JsonSerializer.Serialize(obj, _jsonOptions),
            Encoding.UTF8,
            "application/json"
        );
    }

    private async Task<T?> ReadResponseAsync<T>(HttpResponseMessage response, CancellationToken token = default)
    {
        await EnsureSuccessAsync(response, token);
        return await TryDeserialize<T>(response.Content, token);
    }

    private async Task EnsureSuccessAsync(HttpResponseMessage response, CancellationToken token)
    {
        if (response.StatusCode == HttpStatusCode.Unauthorized ||
            response.StatusCode == HttpStatusCode.Forbidden) {
            //_navigationManager.NavigateTo("/Account/Login", forceLoad: true);
        }

        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var message = await ExtractErrorMessageAsync(response.Content, response.StatusCode, token);
        throw new HttpRequestException(message, null, response.StatusCode);
    }

    private async Task<string> ExtractErrorMessageAsync(HttpContent? content, HttpStatusCode statusCode, CancellationToken token)
    {
        var fallback = $"{(int)statusCode} {statusCode}";
        if (content is null)
        {
            return fallback;
        }

        var raw = await content.ReadAsStringAsync(token);
        if (string.IsNullOrWhiteSpace(raw))
        {
            return fallback;
        }

        try
        {
            using var document = JsonDocument.Parse(raw);
            var root = document.RootElement;

            if (root.TryGetProperty("detail", out var detail) &&
                detail.ValueKind == JsonValueKind.String &&
                !string.IsNullOrWhiteSpace(detail.GetString()))
            {
                return detail.GetString()!;
            }

            if (root.TryGetProperty("title", out var title) &&
                title.ValueKind == JsonValueKind.String &&
                !string.IsNullOrWhiteSpace(title.GetString()))
            {
                return title.GetString()!;
            }
        }
        catch (JsonException)
        {
        }

        return fallback;
    }

    private async Task<T?> TryDeserialize<T>(HttpContent? content, CancellationToken token = default)
    {
        if (content is null) return default;
        if (content.Headers.ContentLength == 0) return default;

        try {
            return await JsonSerializer.DeserializeAsync<T>(await content.ReadAsStreamAsync(token), _jsonOptions, token);
        } catch (Exception) {
            return default;
        }
    }
}
