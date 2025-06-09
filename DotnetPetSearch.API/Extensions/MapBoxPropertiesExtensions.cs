using DotnetPetSearch.API.Models;
using DotnetPetSearch.MapBoxHttpClient;

namespace DotnetPetSearch.API.Extensions;

public static class MapBoxPropertiesExtensions
{
    public static LocationDto ToLocationDto(this MapBoxProperties mapBoxProperties)
    {
        return new LocationDto
        {
            LocationName = mapBoxProperties.PlaceFormatted,
            Zipcode = mapBoxProperties.Name
        };
    }
}