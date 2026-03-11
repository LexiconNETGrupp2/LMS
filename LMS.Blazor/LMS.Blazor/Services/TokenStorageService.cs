using LMS.Shared.DTOs.AuthDtos;
using System.Collections.Concurrent;

namespace LMS.Blazor.Services;

public class TokenStorageService : ITokenStorage
{
    private readonly ConcurrentDictionary<string, TokenDto> _tokenStore = new();
    private readonly ILogger<TokenStorageService> _logger;

    public TokenStorageService(ILogger<TokenStorageService> logger)
    {
        _logger = logger;
    }

    public Task StoreTokensAsync(string userId, TokenDto tokens)
    {
        ArgumentException.ThrowIfNullOrEmpty(userId);
        ArgumentNullException.ThrowIfNull(tokens);

        _tokenStore[userId] = tokens;
        _logger.LogDebug("Stored tokens for user {UserId}", userId);

        return Task.CompletedTask;
    }

    public Task<TokenDto?> GetTokensAsync(string userId)
    {
        ArgumentException.ThrowIfNullOrEmpty(userId);

        _tokenStore.TryGetValue(userId, out var tokens);
        return Task.FromResult(tokens);
    }

    public Task<string?> GetAccessTokenAsync(string userId)
    {
        ArgumentException.ThrowIfNullOrEmpty(userId);

        _tokenStore.TryGetValue(userId, out var tokens);
        return Task.FromResult(tokens?.AccessToken);
    }

    public Task RemoveTokensAsync(string userId)
    {
        ArgumentException.ThrowIfNullOrEmpty(userId);

        _tokenStore.Remove(userId, out _);
        _logger.LogDebug("Removed tokens for user {UserId}", userId);

        return Task.CompletedTask;
    }
}
