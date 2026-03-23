namespace LMS.Blazor.Client.Services;

public interface IApiService
{
    Task<T?> GetAsync<T>(string endpoint, CancellationToken ct = default);
    Task<TResult?> PostAsync<TParam, TResult>(string endpoint, TParam body, CancellationToken token = default);
}