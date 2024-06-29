using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using log4net;

public class OpenRouteService
{
    private static readonly ILog log = LogManager.GetLogger(typeof(OpenRouteService));
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _baseUrl = "https://api.openrouteservice.org";

    public OpenRouteService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["OpenRouteService:ApiKey"];
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
    }

    public async Task<JObject> GetRouteAsync(string start, string end, string profile = "driving-car")
    {
        log.Debug($"Requesting route from {start} to {end} using profile {profile}");
        var requestUrl = $"{_baseUrl}/v2/directions/{profile}?api_key={_apiKey}&start={start}&end={end}";
        log.Debug($"Request URL: {requestUrl}");

        try
        {
            var response = await _httpClient.GetAsync(requestUrl);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            log.Info($"Route retrieved for {start} to {end}");
            return JObject.Parse(content);
        }
        catch (HttpRequestException httpRequestException)
        {
            log.Error($"HttpRequestException: {httpRequestException.Message}");
            if (httpRequestException.InnerException != null)
            {
                log.Error($"Inner Exception: {httpRequestException.InnerException.Message}");
            }
            log.Error($"Request URL: {requestUrl}");
            throw; // Rethrow to allow higher-level handling if needed
        }
        catch (Exception ex)
        {
            log.Error($"General Exception: {ex.Message}");
            if (ex.InnerException != null)
            {
                log.Error($"Inner Exception: {ex.InnerException.Message}");
            }
            log.Error($"Request URL: {requestUrl}");
            throw; // Rethrow to allow higher-level handling if needed
        }
    }

    public async Task<JObject> PostRouteAsync(JArray coordinates, string profile = "driving-car")
    {
        log.Debug($"Requesting route for coordinates using profile {profile}");
        var content = new StringContent(new JObject { ["coordinates"] = coordinates }.ToString(), System.Text.Encoding.UTF8, "application/json");
        var requestUrl = $"{_baseUrl}/v2/directions/{profile}";
        log.Debug($"Request URL: {requestUrl}");

        try
        {
            var response = await _httpClient.PostAsync(requestUrl, content);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            log.Info($"Route retrieved for provided coordinates");
            return JObject.Parse(responseContent);
        }
        catch (HttpRequestException httpRequestException)
        {
            log.Error($"HttpRequestException: {httpRequestException.Message}");
            if (httpRequestException.InnerException != null)
            {
                log.Error($"Inner Exception: {httpRequestException.InnerException.Message}");
            }
            log.Error($"Request URL: {requestUrl}");
            throw; // Rethrow to allow higher-level handling if needed
        }
        catch (Exception ex)
        {
            log.Error($"General Exception: {ex.Message}");
            if (ex.InnerException != null)
            {
                log.Error($"Inner Exception: {ex.InnerException.Message}");
            }
            log.Error($"Request URL: {requestUrl}");
            throw; // Rethrow to allow higher-level handling if needed
        }
    }

    public async Task<JObject> GeocodeAsync(string location)
    {
        log.Debug($"Geocoding location: {location}");
        var requestUrl = $"{_baseUrl}/geocode/search?api_key={_apiKey}&text={location}";
        log.Debug($"Request URL: {requestUrl}");

        try
        {
            var response = await _httpClient.GetAsync(requestUrl);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            log.Info($"Geocode retrieved for location: {location}");
            return JObject.Parse(content);
        }
        catch (HttpRequestException httpRequestException)
        {
            log.Error($"HttpRequestException: {httpRequestException.Message}");
            if (httpRequestException.InnerException != null)
            {
                log.Error($"Inner Exception: {httpRequestException.InnerException.Message}");
            }
            log.Error($"Request URL: {requestUrl}");
            throw; // Rethrow to allow higher-level handling if needed
        }
        catch (Exception ex)
        {
            log.Error($"General Exception: {ex.Message}");
            if (ex.InnerException != null)
            {
                log.Error($"Inner Exception: {ex.InnerException.Message}");
            }
            log.Error($"Request URL: {requestUrl}");
            throw; // Rethrow to allow higher-level handling if needed
        }
    }

    public async Task<(double Longitude, double Latitude)> GetCoordinatesAsync(string location)
    {
        log.Debug($"Getting coordinates for location: {location}");
        try
        {
            var geocodeResult = await GeocodeAsync(location);
            var coordinates = geocodeResult["features"]?[0]?["geometry"]?["coordinates"];
            if (coordinates == null)
            {
                log.Error($"Invalid geocode response format for location: {location}");
                throw new Exception("Invalid geocode response format.");
            }
            log.Info($"Coordinates retrieved for location: {location}");
            return (coordinates[0].ToObject<double>(), coordinates[1].ToObject<double>());
        }
        catch (HttpRequestException httpRequestException)
        {
            log.Error($"HttpRequestException while getting coordinates: {httpRequestException.Message}");
            if (httpRequestException.InnerException != null)
            {
                log.Error($"Inner Exception: {httpRequestException.InnerException.Message}");
            }
            throw; // Rethrow to allow higher-level handling if needed
        }
        catch (Exception ex)
        {
            log.Error($"General Exception while getting coordinates: {ex.Message}");
            if (ex.InnerException != null)
            {
                log.Error($"Inner Exception: {ex.InnerException.Message}");
            }
            throw; // Rethrow to allow higher-level handling if needed
        }
    }
}
