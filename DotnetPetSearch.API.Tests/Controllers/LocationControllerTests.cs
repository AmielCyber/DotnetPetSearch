using DotnetPetSearch.API.Controllers;
using DotnetPetSearch.API.Models;
using DotnetPetSearch.MapBoxHttpClient;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace DotnetPetSearch.API.Tests.Controllers;

public class LocationControllerTests
{
    private readonly IMapBoxClient _mapBoxClient;
    private readonly LocationController _locationController;
    public LocationControllerTests()
    {
        _mapBoxClient = Substitute.For<IMapBoxClient>();
        _locationController = new LocationController(_mapBoxClient);
    }
    
    [Fact]
    public async Task GetLocationByZipCode_ShouldReturnLocationDto_WhenClientReturnsMapBoxProperties()
    {
        // Assert
        var zipCode = "12345";
        _mapBoxClient.GetLocationFromZipCode(Arg.Any<string>()).Returns(GetExpectedMapBoxProperties());

        // Action
        ActionResult<LocationDto> result = await _locationController.GetLocationByZipCodeAsync(zipCode);

        // Assert
        Assert.IsType<ActionResult<LocationDto>>(result);
        Assert.IsType<OkObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetLocationByZipCode_ShouldReturnMappedLocationDto_WhenClientReturnsMapBoxProperties()
    {
        // Assert
        var zipCode = "12345";
        var expectedMapBoxProperties = GetExpectedMapBoxProperties();
        _mapBoxClient.GetLocationFromZipCode(Arg.Any<string>()).Returns(expectedMapBoxProperties);
        

        // Action
        ActionResult<LocationDto> result = await _locationController.GetLocationByZipCodeAsync(zipCode);
        object? value = ((OkObjectResult)result.Result!).Value;
        
        Assert.IsType<LocationDto>(value);
        Assert.Equal(expectedMapBoxProperties.PlaceFormatted, ((LocationDto)value).LocationName);
        Assert.Equal(expectedMapBoxProperties.Name, ((LocationDto)value).Zipcode);
    }
    [Fact]
    public async Task GetLocationByCoordinates_ShouldReturnLocationDto_WhenClientReturnsMapBoxProperties()
    {
        // Assert
        var latitude = 12.34;
        var longitude = 34.56;
        _mapBoxClient.GetLocationFromCoordinates(Arg.Any<double>(), Arg.Any<double>())
            .Returns(GetExpectedMapBoxProperties());

        // Action
        ActionResult<LocationDto> result = await _locationController.GetLocationByCoordinatesAsync(longitude, latitude);

        // Assert
        Assert.IsType<ActionResult<LocationDto>>(result);
        Assert.IsType<OkObjectResult>(result.Result);
    }
    
    [Fact]
    public async Task GetLocationByCoordinates_ShouldReturnMappedLocationDto_WhenClientReturnsMapBoxProperties()
    {
        // Assert
        var latitude = 12.34;
        var longitude = 34.56;
        var expectedMapBoxProperties = GetExpectedMapBoxProperties();
        _mapBoxClient.GetLocationFromCoordinates(Arg.Any<double>(), Arg.Any<double>())
            .Returns(expectedMapBoxProperties);

        // Action
        ActionResult<LocationDto> result = await _locationController.GetLocationByCoordinatesAsync(longitude, latitude);
        object? value = ((OkObjectResult)result.Result!).Value;
        
        // Assert
        Assert.IsType<LocationDto>(value);
        Assert.Equal(expectedMapBoxProperties.PlaceFormatted, ((LocationDto)value).LocationName);
        Assert.Equal(expectedMapBoxProperties.Name, ((LocationDto)value).Zipcode);
    }
    
    [Fact]
    public async Task GetLocationByZipCode_ShouldReturnNotFound_WhenClientReturnsNull()
    {
        // Assert
        var zipCode = "12345";
        _mapBoxClient.GetLocationFromZipCode(Arg.Any<string>()).ReturnsNull();

        // Action
        ActionResult<LocationDto> result = await _locationController.GetLocationByZipCodeAsync(zipCode);

        // Assert
        Assert.IsType<ActionResult<LocationDto>>(result);
        Assert.IsType<NotFoundResult>(result.Result);
    }
    
    [Fact]
    public async Task GetLocationByCoordinates_ShouldReturnNotFound_WhenClientReturnsNull()
    {
        // Assert
        var latitude = 12.34;
        var longitude = 34.56;
        _mapBoxClient.GetLocationFromCoordinates(Arg.Any<double>(), Arg.Any<double>())
            .ReturnsNull();

        // Action
        ActionResult<LocationDto> result = await _locationController.GetLocationByCoordinatesAsync(longitude, latitude);

        // Assert
        Assert.IsType<ActionResult<LocationDto>>(result);
        Assert.IsType<NotFoundResult>(result.Result);
    }
    
    private MapBoxProperties GetExpectedMapBoxProperties()
        => new()
        {
            PlaceFormatted = "San Diego, CA, United States",
            Name = "12345"
        };
}