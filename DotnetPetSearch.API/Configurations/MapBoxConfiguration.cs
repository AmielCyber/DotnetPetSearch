using System.Globalization;
using Microsoft.AspNetCore.WebUtilities;

namespace DotnetPetSearch.API.Configurations;

public class MapBoxConfiguration
{
    public required IEnumerable<KeyValuePair<string, string?>> Options { get; set; }

    public string GetZipCodeQuery(string zipcode)
    {
        IEnumerable<KeyValuePair<string, string?>> zipcodeOptions = Options
            .Prepend(new KeyValuePair<string, string?>("q", zipcode));
            
        return QueryHelpers.AddQueryString($"forward", zipcodeOptions);
    }

    public string GetCoordsQuery(double longitude, double latitude)
    {
        IEnumerable<KeyValuePair<string, string?>> coordsOptions = Options
            .Prepend(new KeyValuePair<string, string?>("latitude", latitude.ToString(CultureInfo.InvariantCulture)))
            .Prepend(new KeyValuePair<string, string?>("longitude", longitude.ToString(CultureInfo.InvariantCulture)));
            
        return QueryHelpers.AddQueryString($"reverse", coordsOptions);
    }
}