using System.Net;
using System.Text.Json;
using DotnetPetSearch.PetFinderHttpClient.Models;
using DotnetPetSearch.PetFinderHttpClient.Services;
using DotnetPetSearch.PetFinderHttpClient.Tests.Fixtures;

namespace DotnetPetSearch.PetFinderHttpClient.Tests.Services;

public class PetFinderClientTests : IClassFixture<PetFinderClientFixture>
{
    private readonly PetFinderClientFixture _fixture;
    private readonly PetFinderClientBuilder _clientBuilder;

    public PetFinderClientTests(PetFinderClientFixture fixture)
    {
        _fixture = fixture;
        _clientBuilder = new PetFinderClientBuilder(_fixture.BaseUri, fixture.ExpectedToken);
    }

    [Fact]
    public async Task GetPetsAsync_ShouldSetAuthenticationRequestHeaders_ToHttpClient()
    {
        // Arrange
        PetFinderClient petFinderClient = _clientBuilder
            .SetExpectHttpRequestUrl("*") // Base Uri and any query
            .SetExpectHttpRequestHeader("Authorization", $"Bearer {_fixture.ExpectedToken.AccessToken}")
            .SetExpectHttpResponse(HttpStatusCode.OK, _fixture.ExpectedPetListResponse)
            .Build();

        // Action
        await petFinderClient.GetPetsAsync(_fixture.ExpectedParameters);

        // Assert all expected http requests and response have been received.
        _clientBuilder.MockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task GetPetsAsync_ShouldPassQueryParametersToRequest()
    {
        Dictionary<string, string> expectedQuery = new()
        {
            { "type", _fixture.ExpectedParameters.Type },
            { "location", _fixture.ExpectedParameters.Location },
            { "page", _fixture.ExpectedParameters.Page.ToString() },
            { "distance", _fixture.ExpectedParameters.Distance.ToString() },
            { "sort", _fixture.ExpectedParameters.SortBy }
        };
        // Arrange
        PetFinderClient petFinderClient = _clientBuilder
            .SetExpectHttpRequestUrl("*")
            .SetExpectHttpQueryRequest(expectedQuery)
            .SetExpectHttpResponse(HttpStatusCode.OK, _fixture.ExpectedPetListResponse)
            .Build();

        // Action
        await petFinderClient.GetPetsAsync(_fixture.ExpectedParameters);

        // Assert all expected http requests and response have been received.
        _clientBuilder.MockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task GetPetsAsync_ShouldPassQueryParametersToRequest_WithDefaultParameters()
    {
        Dictionary<string, string> expectedQuery = new()
        {
            { "type", _fixture.ExpectedParametersWithDefaults.Type },
            { "location", _fixture.ExpectedParametersWithDefaults.Location },
            { "page", _fixture.ExpectedParametersWithDefaults.Page.ToString() },
            { "distance", _fixture.ExpectedParametersWithDefaults.Distance.ToString() },
            { "sort", _fixture.ExpectedParametersWithDefaults.SortBy }
        };
        // Arrange
        PetFinderClient petFinderClient = _clientBuilder
            .SetExpectHttpRequestUrl("*")
            .SetExpectHttpQueryRequest(expectedQuery)
            .SetExpectHttpResponse(HttpStatusCode.OK, _fixture.ExpectedPetListResponse)
            .Build();

        // Action
        await petFinderClient.GetPetsAsync(_fixture.ExpectedParametersWithDefaults);

        // Assert all expected http requests and response have been received.
        _clientBuilder.MockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task GetPetsAsync_ShouldThrowHttpRequestException_WhenResponseIsUnsuccessful()
    {
        // Arrange
        PetFinderClient petFinderClient = _clientBuilder
            .SetExpectHttpRequestUrl("*") // Base Uri and any query
            .SetExpectHttpResponse(HttpStatusCode.InternalServerError, _fixture.ExpectedPetListResponse)
            .Build();

        // Action and Assert
        await Assert.ThrowsAsync<HttpRequestException>(async () =>
            await petFinderClient.GetPetsAsync(_fixture.ExpectedParametersWithDefaults));
        // Assert all expected http requests and response have been received.
        _clientBuilder.MockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task GetPetsAsync_ShouldThrowJsonException_WhenResponseContentFailsToExtractBody()
    {
        // Arrange
        PetFinderClient petFinderClient = _clientBuilder
            .SetExpectHttpRequestUrl("*") // Base Uri and any query
            .SetExpectHttpResponse(HttpStatusCode.OK, null)
            .Build();

        // Action and Assert
        await Assert.ThrowsAsync<JsonException>(async () =>
            await petFinderClient.GetPetsAsync(_fixture.ExpectedParametersWithDefaults));
        // Assert all expected http requests and response have been received.
        _clientBuilder.MockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task GetPetsAsync_ShouldReturn_APetListResponse_WhenResponseIsSuccessful()
    {
        // Arrange
        PetFinderClient petFinderClient = _clientBuilder
            .SetExpectHttpRequestUrl("*") // Base Uri and any query
            .SetExpectHttpResponse(HttpStatusCode.OK, _fixture.ExpectedPetListResponse)
            .Build();

        // Action
        PetFinderPetListResponse petListResponse = await petFinderClient.GetPetsAsync(_fixture.ExpectedParameters);

        // Assert
        Assert.IsType<PetFinderPetListResponse>(petListResponse);
        Assert.Equivalent(_fixture.ExpectedPetListResponse, petListResponse);
        _clientBuilder.MockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task GetSinglePetByIdAsync_ShouldSetAuthenticationRequestHeaders_ToHttpClient()
    {
        // Arrange
        var expectedId = 1;
        PetFinderClient petFinderClient = _clientBuilder
            .SetExpectHttpRequestUrl($"/{expectedId}") // Base Uri with id url
            .SetExpectHttpRequestHeader("Authorization", $"Bearer {_fixture.ExpectedToken.AccessToken}")
            .SetExpectHttpResponse(HttpStatusCode.OK, _fixture.ExpectedPetResponse)
            .Build();

        // Action
        await petFinderClient.GetSinglePetByIdAsync(expectedId);

        // Assert all expected http requests and response have been received.
        _clientBuilder.MockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task GetSinglePetByIdAsync_ShouldPassIdToRequest()
    {
        // Arrange
        var expectedId = 1;
        PetFinderClient petFinderClient = _clientBuilder
            .SetExpectHttpRequestUrl($"/{expectedId}") // Base Uri with id url
            .SetExpectHttpResponse(HttpStatusCode.OK, _fixture.ExpectedPetResponse)
            .Build();

        // Action
        await petFinderClient.GetSinglePetByIdAsync(expectedId);

        // Assert all expected http requests and response have been received.
        _clientBuilder.MockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task GetSinglePetByIdAsync_ReturnNull_WhenResponseStatusCodeIsNotFound()
    {
        // Arrange
        var expectedId = 1;
        PetFinderClient petFinderClient = _clientBuilder
            .SetExpectHttpRequestUrl($"/{expectedId}") // Base Uri with id url
            .SetExpectHttpResponse(HttpStatusCode.NotFound, _fixture.ExpectedPetResponse)
            .Build();

        // Action
        PetFinderPet? pet = await petFinderClient.GetSinglePetByIdAsync(expectedId);

        // Assert 
        Assert.Null(pet);
        _clientBuilder.MockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task GetSinglePetByIdAsync_ShouldThrowHttpRequestException_WhenResponseIsUnsuccessful()
    {
        // Arrange
        var expectedId = 1;
        PetFinderClient petFinderClient = _clientBuilder
            .SetExpectHttpRequestUrl($"/{expectedId}") // Base Uri with id url
            .SetExpectHttpResponse(HttpStatusCode.InternalServerError, _fixture.ExpectedPetResponse)
            .Build();

        // Action and Assert
        await Assert.ThrowsAsync<HttpRequestException>(async () =>
            await petFinderClient.GetSinglePetByIdAsync(expectedId));
        _clientBuilder.MockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task GetSinglePetByIdAsync_ShouldThrowHttpRequestException_WhenResponseContentFailsToExtractBody()
    {
        // Arrange
        var expectedId = 1;
        PetFinderClient petFinderClient = _clientBuilder
            .SetExpectHttpRequestUrl($"/{expectedId}") // Base Uri with id url
            .SetExpectHttpResponse(HttpStatusCode.OK, _fixture.ExpectedEmptyPet)
            .Build();

        // Action and Assert
        await Assert.ThrowsAsync<JsonException>(async () => await petFinderClient.GetSinglePetByIdAsync(expectedId));
        _clientBuilder.MockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task GetSinglePetByIdAsync_ShouldReturn_APetListResponse_WhenResponseIsSuccessful()
    {
        // Arrange
        var expectedId = 1;
        PetFinderClient petFinderClient = _clientBuilder
            .SetExpectHttpRequestUrl($"/{expectedId}") // Base Uri with id url
            .SetExpectHttpResponse(HttpStatusCode.OK, _fixture.ExpectedPetResponse)
            .Build();

        // Action
        PetFinderPet? petResult = await petFinderClient.GetSinglePetByIdAsync(expectedId);

        // Assert
        Assert.IsType<PetFinderPet>(petResult);
        Assert.Equivalent(_fixture.ExpectedPet, petResult);
        _clientBuilder.MockHttp.VerifyNoOutstandingExpectation();
    }
}