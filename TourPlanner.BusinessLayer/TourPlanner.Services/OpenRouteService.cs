using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using log4net;

public class OpenRouteService {
    private static readonly ILog log = LogManager.GetLogger(typeof(OpenRouteService));
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public OpenRouteService(HttpClient httpClient, IConfiguration configuration) {
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

    public async Task<JObject> GeocodeAsync(string location) {
        log.Debug($"Geocoding location: {location}");
        var response = await _httpClient.GetAsync($"https://api.openrouteservice.org/geocode/search?api_key={_apiKey}&text={location}");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        log.Info($"Geocode retrieved for location: {location}");
        return JObject.Parse(content);
    }

    public async Task<(double Longitude, double Latitude)> GetCoordinatesAsync(string location) {
        log.Debug($"Getting coordinates for location: {location}");
        var geocodeResult = await GeocodeAsync(location);
        var coordinates = geocodeResult["features"]?[0]?["geometry"]?["coordinates"];
        if (coordinates == null) {
            log.Error($"Invalid geocode response format for location: {location}");
            throw new Exception("Invalid geocode response format.");
        }
        log.Info($"Coordinates retrieved for location: {location}");
        return (coordinates[0].ToObject<double>(), coordinates[1].ToObject<double>());
    }
}
