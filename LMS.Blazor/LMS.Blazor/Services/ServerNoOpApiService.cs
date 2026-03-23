using LMS.Blazor.Client.Services;

namespace LMS.Blazor.Services;

public class ServerNoOpApiService(ILogger<ServerNoOpApiService> logger) : IApiService
{
    private readonly ILogger<ServerNoOpApiService> _logger = logger;

    public Task DeleteAsync(string endpoint, CancellationToken token = default)
    {
        _logger.LogWarning("ServerNoOpApiService called for: {Endpoint}", endpoint);
        return Task.CompletedTask;
    }

    public Task<T?> GetAsync<T>(string endpoint, CancellationToken ct = default)
    {
        _logger.LogWarning("ServerNoOpApiService called for: {Endpoint}", endpoint);
        return Task.FromResult<T?>(default);
    }

    public Task<TResult?> PatchAsync<TParam, TResult>(string endpoint, TParam body, CancellationToken token = default)
    {
        _logger.LogWarning("ServerNoOpApiService called for: {Endpoint}", endpoint);
        return Task.FromResult<TResult?>(default);
    }

    public Task<TResult?> PostAsync<TParam, TResult>(string endpoint, TParam body, CancellationToken token = default)
    {
        _logger.LogWarning("ServerNoOpApiService called for: {Endpoint}", endpoint);
        return Task.FromResult<TResult?>(default);
    }
}
