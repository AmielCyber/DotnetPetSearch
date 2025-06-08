using DotnetPetSearch.API.MapBoxHttpClient;
using DotnetPetSearch.API.Models;

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