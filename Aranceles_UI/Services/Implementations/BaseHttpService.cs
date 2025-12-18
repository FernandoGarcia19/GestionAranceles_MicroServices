using System.Net.Http.Headers;
using System.Security.Claims;

namespace Aranceles_UI.Services.Implementations;

public abstract class BaseHttpService
{
    protected readonly HttpClient _httpClient;
    protected readonly IHttpContextAccessor _httpContextAccessor;

    protected BaseHttpService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
        
        SetAuthorizationHeader();
    }

    private void SetAuthorizationHeader()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext?.User?.Identity?.IsAuthenticated == true)
        {
            // Get JWT token from user claims
            var jwtToken = httpContext.User.FindFirst("access_token")?.Value;
            if (!string.IsNullOrEmpty(jwtToken))
            {
                _httpClient.DefaultRequestHeaders.Authorization = 
                    new AuthenticationHeaderValue("Bearer", jwtToken);
            }
        }
    }

    protected async Task<HttpResponseMessage> SendAuthenticatedRequestAsync(
        Func<Task<HttpResponseMessage>> request)
    {
        SetAuthorizationHeader();
        return await request();
    }

    protected async Task<T?> GetFromJsonAuthenticatedAsync<T>(string requestUri)
    {
        SetAuthorizationHeader();
        return await _httpClient.GetFromJsonAsync<T>(requestUri);
    }

    protected async Task<HttpResponseMessage> PostAsJsonAuthenticatedAsync<T>(string requestUri, T value)
    {
        SetAuthorizationHeader();
        return await _httpClient.PostAsJsonAsync(requestUri, value);
    }

    protected async Task<HttpResponseMessage> PutAsJsonAuthenticatedAsync<T>(string requestUri, T value)
    {
        SetAuthorizationHeader();
        return await _httpClient.PutAsJsonAsync(requestUri, value);
    }

    protected async Task<HttpResponseMessage> DeleteAuthenticatedAsync(string requestUri)
    {
        SetAuthorizationHeader();
        return await _httpClient.DeleteAsync(requestUri);
    }
}