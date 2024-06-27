using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using log4net;

public class RouteService {
    private static readonly ILog log = LogManager.GetLogger(typeof(RouteService));
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public RouteService(HttpClient httpClient, IConfiguration configuration) {
        _httpClient = httpClient;
        _apiKey = configuration["OpenRouteService:ApiKey"];
    }

    public async Task<JObject> GetRouteAsync(string start, string end) {
        log.Debug($"Requesting route from {start} to {end}");
        var response = await _httpClient.GetAsync($"https://api.openrouteservice.org/v2/directions/driving-car?api_key={_apiKey}&start={start}&end={end}");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        log.Info($"Route retrieved for {start} to {end}");
        return JObject.Parse(content);
    }

    public async Task<List<double[]>> GetCoordinatesAsync(string start, string end) {
        var route = await GetRouteAsync(start, end);
        var coordinates = route["features"][0]["geometry"]["coordinates"]
            .Select(coord => coord.ToObject<double[]>())
            .ToList();

        log.Debug($"Coordinates extracted for route from {start} to {end}");
        return coordinates;
    }
}