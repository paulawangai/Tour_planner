using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

public class RouteService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public RouteService(HttpClient httpClient, IConfiguration configuration)
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

    public async Task<List<double[]>> GetCoordinatesAsync(string start, string end)
    {
        var route = await GetRouteAsync(start, end);
        var coordinates = route["features"][0]["geometry"]["coordinates"]
            .Select(coord => coord.ToObject<double[]>())
            .ToList();

        return coordinates;
    }
}
