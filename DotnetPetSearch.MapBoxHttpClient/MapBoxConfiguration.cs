using System.Globalization;
using Microsoft.AspNetCore.WebUtilities;

namespace DotnetPetSearch.MapBoxHttpClient;

/// <summary>
/// Object for IOptions pattern.
/// The Options will contain keys and values for calling the MapBox API with query values.
/// </summary>
public class MapBoxConfiguration
{
    public required IEnumerable<KeyValuePair<string, string?>> Options { get; set; }

    /// <summary>
    /// Transforms the options key-value pair into string query with the passed zipcode.
    /// </summary>
    /// <param name="zipcode">Zipcode to append to the query.</param>
    /// <returns>A query string with a zipcode and MapBox options.</returns>
    public string GetZipCodeQuery(string zipcode)
    {
        IEnumerable<KeyValuePair<string, string?>> zipcodeOptions = Options
            .Prepend(new KeyValuePair<string, string?>("q", zipcode));

        return QueryHelpers.AddQueryString("forward", zipcodeOptions);
    }

    
    
    /// <summary>
    /// Transforms the options key-value pair into string query with the passed coords.
    /// </summary>
    /// <param name="longitude">Longitude to append the query.</param>
    /// <param name="latitude">Latitude to append to the query</param>
    /// <returns>A query string with longitude, latitude, and MapBox options.</returns>
    ///
    public string GetCoordsQuery(double longitude, double latitude)
    {
        IEnumerable<KeyValuePair<string, string?>> coordsOptions = Options
            .Prepend(new KeyValuePair<string, string?>("latitude", latitude.ToString(CultureInfo.InvariantCulture)))
            .Prepend(new KeyValuePair<string, string?>("longitude", longitude.ToString(CultureInfo.InvariantCulture)));

        return QueryHelpers.AddQueryString("reverse", coordsOptions);
    }
}