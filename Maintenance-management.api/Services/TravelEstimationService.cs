using System.Text.Json;
using Maintenance_management.application.DTOs.TravelEstimation;
using Maintenance_management.application.Interfaces;
using Maintenance_management.domain.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Maintenance_management.api.Services;

public class TravelEstimationService : ITravelEstimationService
{
    private readonly ITechnicianRepository _technicianRepo;
    private readonly IClientRepository _clientRepo;
    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _cache;
    private readonly ILogger<TravelEstimationService> _logger;

    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(30);
    private const string OsrmBaseUrl = "https://router.project-osrm.org/route/v1/driving";
    private const string NominatimBaseUrl = "https://nominatim.openstreetmap.org/search";

    public TravelEstimationService(
        ITechnicianRepository technicianRepo,
        IClientRepository clientRepo,
        IHttpClientFactory httpClientFactory,
        IMemoryCache cache,
        ILogger<TravelEstimationService> logger)
    {
        _technicianRepo = technicianRepo;
        _clientRepo = clientRepo;
        _httpClient = httpClientFactory.CreateClient("TravelEstimation");
        _cache = cache;
        _logger = logger;
    }

    public async Task<TravelEstimationResultDto?> EstimateAsync(Guid technicianId, Guid clientId)
    {
        var cacheKey = $"travel:{technicianId}:{clientId}";
        if (_cache.TryGetValue(cacheKey, out TravelEstimationResultDto? cached))
            return cached;

        var technician = await _technicianRepo.GetByIdAsync(technicianId);
        if (technician is null || technician.IsDeleted) return null;

        var client = await _clientRepo.GetByIdAsync(clientId);
        if (client is null || client.IsDeleted) return null;

        var result = new TravelEstimationResultDto
        {
            TechnicianName = $"{technician.FirstName} {technician.LastName}",
            ClientAddress = client.Address ?? string.Empty,
            TechnicianHasLocation = technician.Latitude.HasValue && technician.Longitude.HasValue
        };

        if (!result.TechnicianHasLocation)
            return result;

        if (string.IsNullOrWhiteSpace(client.Address))
            return result;

        // Geocode client address to coordinates
        var clientCoords = await GeocodeAddressAsync(client.Address);
        if (clientCoords is null)
            return result;

        // Calculate route via OSRM
        var (distanceKm, durationMinutes) = await GetRouteAsync(
            technician.Latitude!.Value, technician.Longitude!.Value,
            clientCoords.Value.Lat, clientCoords.Value.Lon);

        if (distanceKm < 0)
            return result;

        result.DistanceKm = Math.Round(distanceKm, 1);
        result.DurationMinutes = Math.Round(durationMinutes, 0);
        result.FormattedDistance = FormatDistance(distanceKm);
        result.FormattedDuration = FormatDuration(durationMinutes);

        _cache.Set(cacheKey, result, CacheDuration);
        return result;
    }

    private async Task<(double Lat, double Lon)?> GeocodeAddressAsync(string address)
    {
        try
        {
            var url = $"{NominatimBaseUrl}?q={Uri.EscapeDataString(address)}&format=json&limit=1";
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            if (root.ValueKind != JsonValueKind.Array || root.GetArrayLength() == 0)
                return null;

            var first = root[0];
            if (!first.TryGetProperty("lat", out var latEl) ||
                !first.TryGetProperty("lon", out var lonEl))
                return null;

            if (!double.TryParse(latEl.GetString(), System.Globalization.NumberStyles.Float,
                    System.Globalization.CultureInfo.InvariantCulture, out var lat) ||
                !double.TryParse(lonEl.GetString(), System.Globalization.NumberStyles.Float,
                    System.Globalization.CultureInfo.InvariantCulture, out var lon))
                return null;

            return (lat, lon);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to geocode address: {Address}", address);
            return null;
        }
    }

    private async Task<(double DistanceKm, double DurationMinutes)> GetRouteAsync(
        double fromLat, double fromLon, double toLat, double toLon)
    {
        try
        {
            var url = $"{OsrmBaseUrl}/{fromLon},{fromLat};{toLon},{toLat}?overview=false";
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode) return (-1, -1);

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            if (!root.TryGetProperty("code", out var code) || code.GetString() != "Ok")
                return (-1, -1);

            if (!root.TryGetProperty("routes", out var routes) || routes.GetArrayLength() == 0)
                return (-1, -1);

            var route = routes[0];
            var distanceMeters = route.GetProperty("distance").GetDouble();
            var durationSeconds = route.GetProperty("duration").GetDouble();

            return (distanceMeters / 1000.0, durationSeconds / 60.0);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get route from OSRM");
            return (-1, -1);
        }
    }

    private static string FormatDistance(double km) =>
        km >= 1 ? $"{km:F1} km" : $"{km * 1000:F0} m";

    private static string FormatDuration(double minutes)
    {
        var totalMinutes = (int)Math.Round(minutes);
        if (totalMinutes < 60) return $"{totalMinutes} min";
        var hours = totalMinutes / 60;
        var mins = totalMinutes % 60;
        return mins == 0 ? $"{hours}h" : $"{hours}h {mins}min";
    }
}
