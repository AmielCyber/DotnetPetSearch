using System.Globalization;
using Microsoft.AspNetCore.WebUtilities;

namespace DotnetPetSearch.API.Configurations;

public class MapBoxConfiguration
{
    public required IEnumerable<KeyValuePair<string, string?>> Options { get; set; }

    public string GetZipCodeQuery(string zipcode)
    {
        List<KeyValuePair<string, string?>> options =
        [
            new("q", zipcode),
            ..Options,
        ];
        return QueryHelpers.AddQueryString($"forward", options);
    }

    public string GetCoordsQuery(double longitude, double latitude)
    {
        List<KeyValuePair<string, string?>> options =
        [
            new("longitude", longitude.ToString(CultureInfo.InvariantCulture)),
            new("latitude", latitude.ToString(CultureInfo.InvariantCulture)),
            ..Options,
        ];
        return QueryHelpers.AddQueryString($"reverse", options);
    }
}