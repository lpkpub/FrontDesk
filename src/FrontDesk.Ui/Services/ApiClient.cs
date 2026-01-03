using FrontDesk.Shared.Dto;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace FrontDesk.Ui.Services;

public class ApiClient : IApiClient
{
    private readonly HttpClient _http;
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public ApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<VisitResponse> SendVisitRequestAsync(VisitRequest req)
    {
        var response = await _http.PostAsJsonAsync("visit/process", req);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<VisitResponse>(_jsonOptions);
        return result ?? throw new InvalidOperationException("Empty response from API.");
    }
}
