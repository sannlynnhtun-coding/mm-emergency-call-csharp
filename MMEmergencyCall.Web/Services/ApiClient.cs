using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Components;
using MMEmergencyCall.Web.Models;

namespace MMEmergencyCall.Web.Services;

public sealed class ApiClient
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _http;
    private readonly AuthState _authState;
    private readonly NavigationManager _navigation;

    public ApiClient(HttpClient http, AuthState authState, NavigationManager navigation)
    {
        _http = http;
        _authState = authState;
        _navigation = navigation;
    }

    public async Task<ApiResult<AdminSignInModel>> SignInAsync(AdminSigninRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            using var response = await _http.PostAsJsonAsync("api/admin/AdminSignin", request, JsonOptions, cancellationToken);
            var result = await ReadResultAsync<AdminSignInModel>(response, cancellationToken);

            if (response.IsSuccessStatusCode && result.IsSuccess && result.Data is not null)
            {
                _authState.SignIn(result.Data);
            }

            return result;
        }
        catch (Exception ex)
        {
            return ApiResult<AdminSignInModel>.Failure(ex.Message);
        }
    }

    public async Task<ApiResult<DashboardModel>> GetDashboardAsync(CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_authState.Token))
        {
            return ApiResult<DashboardModel>.Failure("Please sign in again.");
        }

        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, "api/Admin/Dashboard");
            request.Headers.TryAddWithoutValidation("Token", _authState.Token);

            using var response = await _http.SendAsync(request, cancellationToken);
            var result = await ReadResultAsync<DashboardModel>(response, cancellationToken);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                _authState.SignOut();
                _navigation.NavigateTo("/login", replace: true);
            }

            return result;
        }
        catch (Exception ex)
        {
            return ApiResult<DashboardModel>.Failure(ex.Message);
        }
    }

    public async Task<ApiResult<T>> GetAdminAsync<T>(string path, CancellationToken cancellationToken = default)
    {
        return await SendAdminAsync<T>(HttpMethod.Get, path, null, cancellationToken);
    }

    public async Task<ApiResult<T>> PatchAdminAsync<T>(string path, CancellationToken cancellationToken = default)
    {
        return await SendAdminAsync<T>(HttpMethod.Patch, path, null, cancellationToken);
    }

    public async Task<ApiResult<T>> PostAdminAsJsonAsync<T>(string path, object body, CancellationToken cancellationToken = default)
    {
        return await SendAdminAsync<T>(HttpMethod.Post, path, JsonContent.Create(body, options: JsonOptions), cancellationToken);
    }

    public async Task<ApiResult<T>> DeleteAdminAsync<T>(string path, CancellationToken cancellationToken = default)
    {
        return await SendAdminAsync<T>(HttpMethod.Delete, path, null, cancellationToken);
    }

    public async Task<ApiResult<T>> PutAdminAsJsonAsync<T>(string path, object body, CancellationToken cancellationToken = default)
    {
        return await SendAdminAsync<T>(HttpMethod.Put, path, JsonContent.Create(body, options: JsonOptions), cancellationToken);
    }

    private async Task<ApiResult<T>> SendAdminAsync<T>(HttpMethod method, string path, HttpContent? content, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_authState.Token))
        {
            content?.Dispose();
            return ApiResult<T>.Failure("Please sign in again.");
        }

        try
        {
            using var request = new HttpRequestMessage(method, path);
            request.Headers.TryAddWithoutValidation("Token", _authState.Token);
            request.Content = content;

            using var response = await _http.SendAsync(request, cancellationToken);
            var result = await ReadResultAsync<T>(response, cancellationToken);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                _authState.SignOut();
                _navigation.NavigateTo("/login", replace: true);
            }

            return result;
        }
        catch (Exception ex)
        {
            return ApiResult<T>.Failure(ex.Message);
        }
    }

    private static async Task<ApiResult<T>> ReadResultAsync<T>(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        try
        {
            var result = await response.Content.ReadFromJsonAsync<ApiResult<T>>(JsonOptions, cancellationToken);
            return result ?? ApiResult<T>.Failure("Empty API response.");
        }
        catch
        {
            return ApiResult<T>.Failure($"{(int)response.StatusCode} {response.ReasonPhrase}");
        }
    }
}
