using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using DotnetPetSearch.API.MapBoxHttpClient;
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

    private readonly Uri _defaultUri = new("https://mapbox.com");
    private readonly MapBoxProperties _expectedMapBoxProperties;
    private readonly IOptions<MapBoxConfiguration> _mapBoxConfiguration;
    private readonly MockHttpMessageHandler _mockHttp;

    public MapBoxClientTests()
    {
        _mockHttp = new MockHttpMessageHandler();
        _expectedMapBoxProperties = new MapBoxProperties
        {
            PlaceFormatted = DefaultLocationName,
            Name = DefaultZipCode
        };
        var mapBoxConfiguration = new MapBoxConfiguration
        {
            Options = new Dictionary<string, string?>
                { { "key", "value" }, { "key2", "value2" }, { "key3", "value3" } }
        };
        _mapBoxConfiguration = Substitute.For<IOptions<MapBoxConfiguration>>();
        _mapBoxConfiguration.Value.Returns(mapBoxConfiguration);
    }

    [Fact]
    public async Task GetLocationFromZipCode_ShouldReturnMapBoxProperty_IfZipcodeIsValid()
    {
        // Arrange
        var request = SetZipcodeRequest(_mockHttp, HttpStatusCode.Accepted);

        var mockHttpClient = _mockHttp.ToHttpClient();
        mockHttpClient.BaseAddress = _defaultUri;


        var mapBoxClient = new MapBoxClient(mockHttpClient, _mapBoxConfiguration);

        // Act
        var result = await mapBoxClient.GetLocationFromZipCode(DefaultZipCode);
        // Assert
        Assert.NotNull(result);
        Assert.IsType<MapBoxProperties>(result);
        Assert.Equivalent(_expectedMapBoxProperties, result);
        Assert.Equal(1, _mockHttp.GetMatchCount(request));
    }

    [Fact]
    public async Task GetLocationFromZipCode_ShouldCallMapBoxUrlFromMapBoxConfigUrl()
    {
        // Arrange
        var request = SetZipcodeRequest(_mockHttp, HttpStatusCode.Accepted);

        var mockHttpClient = _mockHttp.ToHttpClient();
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
        var request = SetZipcodeRequest(_mockHttp, HttpStatusCode.Accepted);

        var mockHttpClient = _mockHttp.ToHttpClient();
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
        var request = SetZipcodeRequest(_mockHttp, HttpStatusCode.Accepted);

        var mockHttpClient = _mockHttp.ToHttpClient();
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
    [InlineData(HttpStatusCode.NotFound)]
    public async Task GetLocationFromZipCode_ShouldReturnNull_IfClientRespondsWithNotFound(HttpStatusCode statusCode)
    {
        // Arrange
        SetZipcodeRequest(_mockHttp, statusCode);
        var mockHttpClient = _mockHttp.ToHttpClient();
        mockHttpClient.BaseAddress = _defaultUri;

        var mapBoxClient = new MapBoxClient(mockHttpClient, _mapBoxConfiguration);

        // Action
        var result = await mapBoxClient.GetLocationFromZipCode(DefaultZipCode);
        Assert.Null(result);
    }

    [Theory]
    [InlineData(HttpStatusCode.BadRequest)]
    [InlineData(HttpStatusCode.Unauthorized)]
    [InlineData(HttpStatusCode.Forbidden)]
    [InlineData(HttpStatusCode.ServiceUnavailable)]
    public async Task GetLocationFromZipCode_ShouldThrow_HttpRequestException_IfStatusCodeIsUnsuccessful(
        HttpStatusCode statusCode
    )
    {
        // Arrange
        SetZipcodeRequest(_mockHttp, statusCode);

        var mockHttpClient = _mockHttp.ToHttpClient();
        mockHttpClient.BaseAddress = _defaultUri;

        var mapBoxClient = new MapBoxClient(mockHttpClient, _mapBoxConfiguration);

        // Act and Assert
        var exception =
            await Assert.ThrowsAsync<HttpRequestException>(() =>
                mapBoxClient.GetLocationFromZipCode(DefaultZipCode));
        _mockHttp.Expect(_defaultUri.ToString()).Respond(statusCode);
        Assert.Equal(statusCode, exception.StatusCode);
    }

    [Fact]
    public async Task GetLocationFromCoordinates_ShouldReturnMapBoxProperty_IfCoordinatesIsValid()
    {
        // Arrange
        var request = SetCoordinatesRequest(_mockHttp, HttpStatusCode.Accepted);

        var mockHttpClient = _mockHttp.ToHttpClient();
        mockHttpClient.BaseAddress = _defaultUri;

        var mapBoxClient = new MapBoxClient(mockHttpClient, _mapBoxConfiguration);

        // Act
        var result = await mapBoxClient.GetLocationFromCoordinates(DefaultLongitude, DefaultLatitude);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<MapBoxProperties>(result);
        Assert.Equivalent(_expectedMapBoxProperties, result);
        Assert.Equal(1, _mockHttp.GetMatchCount(request));
    }

    [Fact]
    public async Task GetLocationFromCoordinates_ShouldCallMapBoxUrlFromMapBoxConfigUrl()
    {
        // Arrange
        var request = SetCoordinatesRequest(_mockHttp, HttpStatusCode.Accepted);

        var mockHttpClient = _mockHttp.ToHttpClient();
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
        var request = SetCoordinatesRequest(_mockHttp, HttpStatusCode.Accepted);

        var mockHttpClient = _mockHttp.ToHttpClient();
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
        var request = SetCoordinatesRequest(_mockHttp, HttpStatusCode.Accepted);

        var mockHttpClient = _mockHttp.ToHttpClient();
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
    [InlineData(HttpStatusCode.Unauthorized)]
    [InlineData(HttpStatusCode.Forbidden)]
    [InlineData(HttpStatusCode.ServiceUnavailable)]
    public async Task GetLocationFromCoordinates_ShouldThrow_HttpRequestException_IfStatusCodeIsUnsuccessful(
        HttpStatusCode statusCode
    )
    {
        // Arrange
        SetCoordinatesRequest(_mockHttp, statusCode);

        var mockHttpClient = _mockHttp.ToHttpClient();
        mockHttpClient.BaseAddress = _defaultUri;

        var mapBoxClient = new MapBoxClient(mockHttpClient, _mapBoxConfiguration);

        // Act
        var exception = await Assert.ThrowsAsync<HttpRequestException>(() =>
            mapBoxClient.GetLocationFromCoordinates(DefaultLongitude, DefaultLatitude));

        // Assert
        Assert.IsType<HttpRequestException>(exception);
        _mockHttp.Expect(_defaultUri.ToString()).Respond(statusCode);
        Assert.Equal(statusCode, exception.StatusCode);
    }

    [Theory]
    [InlineData(HttpStatusCode.NotFound)]
    public async Task GetLocationFromCoords_ShouldReturnNull_IfClientRespondsWithNotFound(HttpStatusCode statusCode)
    {
        // Arrange
        SetCoordinatesRequest(_mockHttp, statusCode);
        var mockHttpClient = _mockHttp.ToHttpClient();
        mockHttpClient.BaseAddress = _defaultUri;

        var mapBoxClient = new MapBoxClient(mockHttpClient, _mapBoxConfiguration);

        // Action
        var result = await mapBoxClient.GetLocationFromCoordinates(DefaultLongitude, DefaultLatitude);
        Assert.Null(result);
    }

    private IMockedRequest SetZipcodeRequest(MockHttpMessageHandler mockHttpMessageHandler, HttpStatusCode statusCode)
    {
        var expectedUriCall = $"{_defaultUri}forward";
        if (statusCode == HttpStatusCode.Accepted)
            return
                mockHttpMessageHandler
                    .When(expectedUriCall)
                    .WithQueryString("q", DefaultZipCode)
                    .Respond(HttpStatusCode.Accepted, JsonContent.Create(new
                    {
                        features = new List<object>
                        {
                            new
                            {
                                properties = new
                                {
                                    place_formatted = DefaultLocationName,
                                    name = DefaultZipCode
                                }
                            }
                        }
                    }));

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
            return
                mockHttpMessageHandler
                    .When(expectedUriCall)
                    .WithQueryString("longitude", DefaultLongitude.ToString(CultureInfo.InvariantCulture))
                    .WithQueryString("latitude", DefaultLatitude.ToString(CultureInfo.InvariantCulture))
                    .Respond(HttpStatusCode.Accepted, JsonContent.Create(new
                    {
                        features = new List<object>
                        {
                            new
                            {
                                properties = new
                                {
                                    place_formatted = DefaultLocationName,
                                    name = DefaultZipCode
                                }
                            }
                        }
                    }));

        return mockHttpMessageHandler
            .When(expectedUriCall)
            .WithQueryString("longitude", DefaultLongitude.ToString(CultureInfo.InvariantCulture))
            .WithQueryString("latitude", DefaultLatitude.ToString(CultureInfo.InvariantCulture))
            .Respond(statusCode);
    }
}