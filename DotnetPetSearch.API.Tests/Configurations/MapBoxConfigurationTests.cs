using DotnetPetSearch.API.Configurations;

namespace DotnetPetSearch.API.Tests.Configurations;

public class MapBoxConfigurationTests
{
    private const string DefaultZipcode = "92101";
    private const double DefaultLongitude = 117.1647;
    private const double DefaultLatitude = 32.7270;

    private readonly IEnumerable<KeyValuePair<string, string?>> _expectedOptions;
    private readonly MapBoxConfiguration _mapBoxConfiguration;

    public MapBoxConfigurationTests()
    {
        // Arrange
        _expectedOptions = new Dictionary<string, string?>()
            { { "key", "value" }, { "key2", "value2" }, { "key3", "value3" } };
        _mapBoxConfiguration = new MapBoxConfiguration() { Options = _expectedOptions };
    }

    [Fact]
    public void GetZipCodeQuery_ShouldReturn_ForwardUri()
    {
        string actualQuery = _mapBoxConfiguration.GetZipCodeQuery(DefaultZipcode);  // Action
        Assert.Contains("forward?", actualQuery);                                   // Assert
    }

    [Theory]
    [InlineData("92101")]
    [InlineData("90210")]
    public void GetZipCodeQuery_ShouldReturn_ExpectedZipcodeQuery(string expectedZipcode)
    {
        string actualQuery = _mapBoxConfiguration.GetZipCodeQuery(expectedZipcode);
        Assert.Contains($"q={expectedZipcode}", actualQuery);
    }

    [Fact]
    public void GetZipCodeQuery_ShouldReturn_ExpectedQueryOptions()
    {
        string actualQuery = _mapBoxConfiguration.GetZipCodeQuery(DefaultZipcode);
        foreach (KeyValuePair<string, string?> query in _expectedOptions)
            Assert.Contains($"{query.Key}={query.Value}", actualQuery);
    }
    
    [Fact]
    public void GetCoordsQuery_ShouldReturn_ReverseUri()
    {
        string actualQuery = _mapBoxConfiguration.GetCoordsQuery(DefaultLongitude, DefaultLatitude);
        Assert.Contains("reverse?", actualQuery);                                   
    }
    
    [Theory]
    [InlineData(1.123, 85.333)]
    [InlineData(3.14, 8.332)]
    public void GetZipCodeQuery_ShouldReturn_ExpectedCoordsQuery(double expectedLongitude, double expectedLatitude)
    {
        string actualQuery = _mapBoxConfiguration.GetCoordsQuery(expectedLongitude, expectedLatitude);
        Assert.Contains($"longitude={expectedLongitude}", actualQuery);
        Assert.Contains($"latitude={expectedLatitude}", actualQuery);
    }
    
    [Fact]
    public void GetCoordsQuery_ShouldReturn_ExpectedQueryOptions()
    {
        string actualQuery = _mapBoxConfiguration.GetCoordsQuery(DefaultLongitude, DefaultLatitude);
        foreach (KeyValuePair<string, string?> query in _expectedOptions)
            Assert.Contains($"{query.Key}={query.Value}", actualQuery);
    }
}