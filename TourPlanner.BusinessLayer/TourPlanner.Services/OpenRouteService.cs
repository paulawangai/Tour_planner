// OpenRouteService.cs
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

public class OpenRouteService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public OpenRouteService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["OpenRouteService:ApiKey"];
    }

    public async Task<JObject> GetRouteAsync(string start, string end)
    {
        var response = await _httpClient.GetAsync($"https://api.openrouteservice.org/v2/directions/driving-car?api_key={_apiKey}&start={start}&end={end}");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        return JObject.Parse(content);
    }
}
