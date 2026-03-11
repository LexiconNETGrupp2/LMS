using LMS.Blazor.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        return null!;
    }
}
