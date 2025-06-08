namespace DotnetPetSearch.API.MapBoxHttpClient;

/// <summary>
///     MapBox contract to get a displayable location name and zipcode from either
///     a zipcode or coordinates.
/// </summary>
public interface IMapBoxClient
{
    public Task<MapBoxProperties?> GetLocationFromZipCode(string zipcode);
    public Task<MapBoxProperties?> GetLocationFromCoordinates(double longitude, double latitude);
}