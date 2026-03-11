using LMS.Blazor.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace LMS.Blazor.Controllers;

[Route("api/proxy")]
[ApiController]
[Authorize]
public class ProxyController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ITokenStorage _tokenStorage;
    private readonly ILogger<ProxyController> _logger;

    public ProxyController(
        IHttpClientFactory httpClientFactory,
        ITokenStorage tokenStorage,
        ILogger<ProxyController> logger)
    {
        _httpClientFactory = httpClientFactory;
        _tokenStorage = tokenStorage;
        _logger = logger;
    }

    [HttpGet("{**endpoint}")]
    public async Task<IActionResult> ProxyEndpoint(string endpoint, CancellationToken cancellationToken)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
            return Unauthorized("User id not found");

        var accestoken = await _tokenStorage.GetAccessTokenAsync(userId);
        if(string.IsNullOrEmpty(accestoken))
            return Unauthorized("No accesstoken found");

        var client = _httpClientFactory.CreateClient("LmsApiClient");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accestoken);

        var response = await client.GetAsync(endpoint);

        var content = await response.Content.ReadAsStringAsync();
        return StatusCode((int)response.StatusCode, content);

    }
}
