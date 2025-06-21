using DotnetPetSearch.API.Controllers;
using DotnetPetSearch.API.Models;
using DotnetPetSearch.MapBoxHttpClient;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;

namespace DotnetPetSearch.API.Tests.Controllers;

public class LocationControllerTests
{
    private IMapBoxClient _mapBoxClient;
    private MapBoxProperties _expectedMapBoxProperties;
    private LocationController _controller;

    public LocationControllerTests()
    {
        _expectedMapBoxProperties = new MapBoxProperties
        {
            PlaceFormatted = "San Diego, CA, 92101",
            Name = "92101"
        };
        _mapBoxClient = Substitute.For<IMapBoxClient>();
        _controller = new LocationController(_mapBoxClient);
    }

    [Fact]
    public async Task GetLocationByZipCode_ShouldReturnLocationDto_WhenClientReturnsMapBoxProperties()
    {
        // Assert
        var zipCode = "12345";
        _mapBoxClient.GetLocationFromZipCode(Arg.Any<string>()).Returns(_expectedMapBoxProperties);

        // Action
        ActionResult<LocationDto> result = await _controller.GetLocationByZipCodeAsync(zipCode);

        // Assert
        Assert.IsType<ActionResult<LocationDto>>(result);
        Assert.IsType<OkObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetLocationByCoordinates_ShouldReturnLocationDto_WhenClientReturnsMapBoxProperties()
    {
        // Assert
        var latitude = 12.34;
        var longitude = 34.56;
        _mapBoxClient.GetLocationFromCoordinates(Arg.Any<double>(), Arg.Any<double>())
            .Returns(_expectedMapBoxProperties);

        // Action
        ActionResult<LocationDto> result = await _controller.GetLocationByCoordinatesAsync(longitude, latitude);

        // Assert
        Assert.IsType<ActionResult<LocationDto>>(result);
        Assert.IsType<OkObjectResult>(result.Result);
    }
}