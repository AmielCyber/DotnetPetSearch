using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using DotnetPetSearch.API.Configurations;
using DotnetPetSearch.API.MapBoxHttpClient;
using DotnetPetSearch.API.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Options;
using NSubstitute;
using RichardSzalay.MockHttp;

namespace DotnetPetSearch.API.Tests.MapBoxHttpClient;

public class MapBoxClientTests
{
    private const string DefaultLocationName = "San Diego";
    private const string DefaultZipCode = "92101";
    private const double DefaultLatitude = 32.7270;
    private const double DefaultLongitude = 117.1647;
    
    private readonly Uri _defaultUri = new Uri("https://mapbox.com");
    private readonly MockHttpMessageHandler _mockHttp;
    private readonly IOptions<MapBoxConfiguration> _mapBoxConfiguration;
    private readonly LocationDto _expectedDefaultLocationDto;

    public MapBoxClientTests()
    {
        _mockHttp = new MockHttpMessageHandler();
        _expectedDefaultLocationDto = new LocationDto()
        {
            LocationName = DefaultLocationName,
            Zipcode = DefaultZipCode,
        };

        var mapBoxConfiguration = new MapBoxConfiguration
        {
           Options = new Dictionary<string, string?>()
            { { "key", "value" }, { "key2", "value2" }, { "key3", "value3" }}
        };
        _mapBoxConfiguration = Substitute.For<IOptions<MapBoxConfiguration>>();
        _mapBoxConfiguration.Value.Returns(mapBoxConfiguration);
    }
    
    [Fact]
    public async Task GetLocationFromZipCode_ShouldReturnALocationDto_IfZipcodeIsValid()
    {
        // Arrange
        var request = SetZipcodeRequest(_mockHttp, HttpStatusCode.Accepted);

        HttpClient mockHttpClient = _mockHttp.ToHttpClient();
        mockHttpClient.BaseAddress = _defaultUri;


        var mapBoxClient = new MapBoxClient(mockHttpClient, _mapBoxConfiguration);

        // Act
        var result = await mapBoxClient.GetLocationFromZipCode(DefaultZipCode);
        LocationDto? locationDto = Assert.IsType<Ok<LocationDto>>(result.Result).Value;
        // Assert
        Assert.NotNull(locationDto);
        Assert.IsType<LocationDto>(locationDto);
        Assert.Equivalent(_expectedDefaultLocationDto, locationDto);
        Assert.Equal(1, _mockHttp.GetMatchCount(request));
    }
    
    [Fact]
    public async Task GetLocationFromZipCode_ShouldCallMapBoxUrlFromMapBoxConfigUrl()
    {
        // Arrange
        IMockedRequest request = SetZipcodeRequest(_mockHttp, HttpStatusCode.Accepted);

        HttpClient mockHttpClient = _mockHttp.ToHttpClient();
        mockHttpClient.BaseAddress = _defaultUri;

        var mapBoxClient = new MapBoxClient(mockHttpClient, _mapBoxConfiguration);

        // Act
        await mapBoxClient.GetLocationFromZipCode(DefaultZipCode);

        // Assert
        Assert.Equal(1, _mockHttp.GetMatchCount(request));
        _mockHttp.Expect(HttpMethod.Get, _defaultUri.ToString());
    }
    
    [Fact]
    public async Task GetLocationFromZipCode_ShouldCallMapBoxUrlWithMapBoxConfigQuery()
    {
        // Arrange
        IMockedRequest request = SetZipcodeRequest(_mockHttp, HttpStatusCode.Accepted);

        HttpClient mockHttpClient = _mockHttp.ToHttpClient();
        mockHttpClient.BaseAddress = _defaultUri;

        var mapBoxClient = new MapBoxClient(mockHttpClient, _mapBoxConfiguration);

        // Act
        await mapBoxClient.GetLocationFromZipCode(DefaultZipCode);

        // Assert
        Assert.Equal(1, _mockHttp.GetMatchCount(request));
        _mockHttp.Expect(_defaultUri.ToString())
            .WithQueryString(_mapBoxConfiguration.Value.GetZipCodeQuery(DefaultZipCode));
    }
    
    [Fact]
    public async Task GetLocationFromZipCode_ShouldCallMapBoxUrlWithPassedZipcode()
    {
        // Arrange
        IMockedRequest request = SetZipcodeRequest(_mockHttp, HttpStatusCode.Accepted);

        HttpClient mockHttpClient = _mockHttp.ToHttpClient();
        mockHttpClient.BaseAddress = _defaultUri;

        var mapBoxClient = new MapBoxClient(mockHttpClient, _mapBoxConfiguration);

        // Act
        await mapBoxClient.GetLocationFromZipCode(DefaultZipCode);

        // Assert
        Assert.Equal(1, _mockHttp.GetMatchCount(request));
        _mockHttp.Expect(_defaultUri.ToString())
            .WithContent(DefaultZipCode);
    }
    
    [Theory]
    [InlineData(HttpStatusCode.BadRequest)]
    [InlineData(HttpStatusCode.NotFound)]
    public async Task GetLocationFromZipCode_ShouldReturnCorrectError_ForHandledErrors(
        HttpStatusCode statusCode
    )
    {
        // Arrange
        SetZipcodeRequest(_mockHttp, statusCode);

        HttpClient mockHttpClient = _mockHttp.ToHttpClient();
        mockHttpClient.BaseAddress = _defaultUri;

        var mapBoxClient = new MapBoxClient(mockHttpClient, _mapBoxConfiguration);

        // Act
        Results<Ok<LocationDto>, ProblemHttpResult> result = await mapBoxClient.GetLocationFromZipCode(DefaultZipCode);

        ProblemHttpResult problemResult = Assert.IsType<ProblemHttpResult>(result.Result);
        // Assert
        _mockHttp.Expect(_defaultUri.ToString()).Respond(statusCode);
        Assert.Equal((int)statusCode, problemResult.StatusCode);
    }
    
    [Theory]
    [InlineData(HttpStatusCode.Unauthorized)]
    [InlineData(HttpStatusCode.Forbidden)]
    [InlineData(HttpStatusCode.ServiceUnavailable)]
    public async Task GetLocationFromZipCode_ShouldThrow_ForUnhandledErrorsResponses(HttpStatusCode statusCode)
    {
        // Arrange
        SetZipcodeRequest(_mockHttp, HttpStatusCode.Forbidden);

        HttpClient mockHttpClient = _mockHttp.ToHttpClient();
        mockHttpClient.BaseAddress = _defaultUri;

        var mapBoxClient = new MapBoxClient(mockHttpClient, _mapBoxConfiguration);

        // Act
        await Assert.ThrowsAsync<Exception>(
            async () => await mapBoxClient.GetLocationFromZipCode(DefaultZipCode)
        );
    }
    
    [Fact]
    public async Task GetLocationFromCoordinates_ShouldReturnALocationDto()
    {
        // Arrange
        var request = SetCoordinatesRequest(_mockHttp, HttpStatusCode.Accepted);

        HttpClient mockHttpClient = _mockHttp.ToHttpClient();
        mockHttpClient.BaseAddress = _defaultUri;

        var mapBoxClient = new MapBoxClient(mockHttpClient, _mapBoxConfiguration);

        // Act
        Results<Ok<LocationDto>, ProblemHttpResult> result =
            await mapBoxClient.GetLocationFromCoordinates(DefaultLongitude, DefaultLatitude);
        LocationDto? locationDto = Assert.IsType<Ok<LocationDto>>(result.Result).Value;

        // Assert
        Assert.NotNull(locationDto);
        Assert.IsType<LocationDto>(locationDto);
        Assert.Equivalent(_expectedDefaultLocationDto, locationDto);
        Assert.Equal(1, _mockHttp.GetMatchCount(request));
    }

    [Fact]
    public async Task GetLocationFromCoordinates_ShouldCallMapBoxUrlFromMapBoxConfigUrl()
    {
        // Arrange
        IMockedRequest request = SetCoordinatesRequest(_mockHttp, HttpStatusCode.Accepted);

        HttpClient mockHttpClient = _mockHttp.ToHttpClient();
        mockHttpClient.BaseAddress = _defaultUri;

        var mapBoxClient = new MapBoxClient(mockHttpClient, _mapBoxConfiguration);

        // Act
        await mapBoxClient.GetLocationFromCoordinates(DefaultLongitude, DefaultLatitude);

        // Assert
        Assert.Equal(1, _mockHttp.GetMatchCount(request));
        _mockHttp.Expect(_defaultUri.ToString());
    }

    [Fact]
    public async Task GetLocationFromCoordinates_ShouldCallMapBoxUrlMapBoxConfigQuery()
    {
        // Arrange
        IMockedRequest request = SetCoordinatesRequest(_mockHttp, HttpStatusCode.Accepted);

        HttpClient mockHttpClient = _mockHttp.ToHttpClient();
        mockHttpClient.BaseAddress = _defaultUri;

        var mapBoxClient = new MapBoxClient(mockHttpClient, _mapBoxConfiguration);

        // Act
        await mapBoxClient.GetLocationFromCoordinates(DefaultLongitude, DefaultLatitude);

        // Assert
        Assert.Equal(1, _mockHttp.GetMatchCount(request));
        _mockHttp.Expect(_defaultUri.ToString())
            .WithQueryString(_mapBoxConfiguration.Value.GetZipCodeQuery(DefaultZipCode));
    }

    [Fact]
    public async Task GetLocationFromCoordinates_ShouldCallMapBoxUrlWithPassedCoordinates()
    {
        // Arrange
        IMockedRequest request = SetCoordinatesRequest(_mockHttp, HttpStatusCode.Accepted);

        HttpClient mockHttpClient = _mockHttp.ToHttpClient();
        mockHttpClient.BaseAddress = _defaultUri;

        var mapBoxClient = new MapBoxClient(mockHttpClient, _mapBoxConfiguration);

        // Act
        await mapBoxClient.GetLocationFromCoordinates(DefaultLongitude, DefaultLatitude);

        // Assert
        Assert.Equal(1, _mockHttp.GetMatchCount(request));
        _mockHttp.Expect(_defaultUri.ToString())
            .WithContent(DefaultLongitude.ToString(CultureInfo.InvariantCulture))
            .WithContent(DefaultLatitude.ToString(CultureInfo.InvariantCulture));
    }

    [Theory]
    [InlineData(HttpStatusCode.BadRequest)]
    [InlineData(HttpStatusCode.NotFound)]
    public async Task GetLocationFromCoordinates_ShouldReturnCorrectError_IfResultIsUnsuccessful(
        HttpStatusCode statusCode
    )
    {
        // Arrange
        SetCoordinatesRequest(_mockHttp, statusCode);

        HttpClient mockHttpClient = _mockHttp.ToHttpClient();
        mockHttpClient.BaseAddress = _defaultUri;

        var mapBoxClient = new MapBoxClient(mockHttpClient, _mapBoxConfiguration);

        // Act
        Results<Ok<LocationDto>, ProblemHttpResult> result =
            await mapBoxClient.GetLocationFromCoordinates(DefaultLongitude, DefaultLatitude);

        ProblemHttpResult problemResult = Assert.IsType<ProblemHttpResult>(result.Result);

        // Assert
        _mockHttp.Expect(_defaultUri.ToString()).Respond(statusCode);
        Assert.Equal((int)statusCode, problemResult.StatusCode);
    }

    [Theory]
    [InlineData(HttpStatusCode.Unauthorized)]
    [InlineData(HttpStatusCode.Forbidden)]
    [InlineData(HttpStatusCode.ServiceUnavailable)]
    public async Task GetLocationFromCoordinates_ShouldThrow_ForUnhandledErrorsResponses(HttpStatusCode statusCode)
    {
        // Arrange
        SetCoordinatesRequest(_mockHttp, HttpStatusCode.Forbidden);

        HttpClient mockHttpClient = _mockHttp.ToHttpClient();
        mockHttpClient.BaseAddress = _defaultUri;

        var mapBoxClient = new MapBoxClient(mockHttpClient, _mapBoxConfiguration);

        // Act
        await Assert.ThrowsAsync<Exception>(
            async () => await mapBoxClient.GetLocationFromCoordinates(DefaultLongitude, DefaultLatitude)
        );
    }

    private IMockedRequest SetZipcodeRequest(MockHttpMessageHandler mockHttpMessageHandler, HttpStatusCode statusCode)
    {
        var expectedUriCall = $"{_defaultUri}forward";
        if (statusCode == HttpStatusCode.Accepted)
        {
            return
                mockHttpMessageHandler
                    .When(expectedUriCall)
                    .WithQueryString("q", DefaultZipCode)
                    .Respond(HttpStatusCode.Accepted, JsonContent.Create(new
                    {
                        features = new List<object>()
                        {
                            new
                            {
                                properties = new {
                                    place_formatted = DefaultLocationName,
                                    name = DefaultZipCode
                                }
                            }
                        },
                    }));
        }

        return mockHttpMessageHandler
            .When(expectedUriCall)
            .WithQueryString("q", DefaultZipCode)
            .Respond(statusCode);
    }
    
    private IMockedRequest SetCoordinatesRequest(MockHttpMessageHandler mockHttpMessageHandler,
        HttpStatusCode statusCode)
    {
        var expectedUriCall = $"{_defaultUri}reverse";
        if (statusCode == HttpStatusCode.Accepted)
        {
            return
                mockHttpMessageHandler
                    .When(expectedUriCall)
                    .WithQueryString("longitude", DefaultLongitude.ToString(CultureInfo.InvariantCulture))
                    .WithQueryString("latitude", DefaultLatitude.ToString(CultureInfo.InvariantCulture))
                    .Respond(HttpStatusCode.Accepted, JsonContent.Create(new
                    {
                        features = new List<object>()
                        {
                            new
                            {
                                properties = new {
                                    place_formatted = DefaultLocationName,
                                    name = DefaultZipCode
                                }
                            }
                        },
                    }));
        }

        return mockHttpMessageHandler
            .When(expectedUriCall)
            .WithQueryString("longitude", DefaultLongitude.ToString(CultureInfo.InvariantCulture))
            .WithQueryString("latitude", DefaultLatitude.ToString(CultureInfo.InvariantCulture))
            .Respond(statusCode);
    }

}