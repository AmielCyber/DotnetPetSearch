using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using DotnetPetSearch.Data.Configurations;
using DotnetPetSearch.Data.Entities;
using DotnetPetSearch.Data.Models;
using DotnetPetSearch.PetFinderHttpClient.Services;
using DotnetPetSearch.PetFinderHttpClient.Tests.TestData;
using Microsoft.Extensions.Options;
using RichardSzalay.MockHttp;

namespace DotnetPetSearch.PetFinderHttpClient.Tests.Services;

public class PetFinderTokenClientTests
{
    private readonly MockHttpMessageHandler _mockHttp;

    private readonly PetFinderToken _expectedPetFinderToken = new()
    {
        AccessToken = "access_token",
        ExpiresIn = DateTime.Now
    };

    private readonly TokenResponse _expectedTokenResponse;
    private readonly Uri _baseUri = new("https://petfinder.com/v2/token");

    private readonly IOptions<PetFinderCredentials> _credentials = Options.Create(new PetFinderCredentials
    {
        ClientId = "Client",
        ClientSecret = "Secret"
    });

    public PetFinderTokenClientTests()
    {
        _mockHttp = new MockHttpMessageHandler();
        _expectedTokenResponse = new TokenResponse
        {
            TokenType = "Type",
            ExpiresIn = 3600,
            AccessToken = _expectedPetFinderToken.AccessToken
        };
    }


    [Fact]
    public async Task TryGetTokenFromApiAsync_ShouldReturnToken_IfRequestIsSuccessful()
    {
        // Arrange
        _mockHttp
            .When(_baseUri.ToString())
            .Respond(HttpStatusCode.Accepted, JsonContent.Create(_expectedTokenResponse));
        IPetFinderTokenClient petFinderTokenClient = new PetFinderTokenClient(CreateHttpClient(), _credentials);

        // Action
        PetFinderToken? result = await petFinderTokenClient.TryGetTokenFromApiAsync(CancellationToken.None);

        // Assert
        Assert.IsType<PetFinderToken>(result);
        Assert.Equal(_expectedPetFinderToken.AccessToken, result.AccessToken);
    }

    [Fact]
    public async Task TryGetTokenFromApiAsync_ShouldCallPetFinderApiWithClientCredentialsInRequestBody()
    {
        // Arrange
        _mockHttp
            .When(_baseUri.ToString())
            .Respond(HttpStatusCode.Accepted, JsonContent.Create(_expectedTokenResponse))
            .WithJsonContent(_credentials.Value);
        IPetFinderTokenClient petFinderTokenClient = new PetFinderTokenClient(CreateHttpClient(), _credentials);

        // Action
        _ = await petFinderTokenClient.TryGetTokenFromApiAsync(CancellationToken.None);

        // Assert
        _mockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task TryGetTokenFromApiAsync_ShouldReturnTokenWithFutureExpiration_IfRequestIsSuccessful()
    {
        // Arrange
        _mockHttp
            .When(_baseUri.ToString())
            .Respond(HttpStatusCode.Accepted, JsonContent.Create(_expectedTokenResponse));
        IPetFinderTokenClient petFinderTokenClient = new PetFinderTokenClient(CreateHttpClient(), _credentials);

        // Action
        PetFinderToken? result = await petFinderTokenClient.TryGetTokenFromApiAsync(CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(DateTime.Now < result.ExpiresIn);
    }

    [Theory]
    [ClassData(typeof(UnsuccessfulStatusCodes))]
    public async Task TryGetTokenFromApiAsync_ShouldThrowHttpRequestException_IfRequestIsNotSuccessful(
        HttpStatusCode statusCode)
    {
        // Arrange
        _mockHttp
            .When(_baseUri.ToString())
            .Respond(statusCode);
        IPetFinderTokenClient petFinderTokenClient = new PetFinderTokenClient(CreateHttpClient(), _credentials);

        // Action + Assert
        await Assert.ThrowsAsync<HttpRequestException>(async () =>
            await petFinderTokenClient.TryGetTokenFromApiAsync(CancellationToken.None));
    }

    [Fact]
    public async Task TryGetTokenFromApiAsync_ShouldJsonException_IfFailedToReadResponseBody()
    {
        // Arrange
        _mockHttp
            .When(_baseUri.ToString())
            .Respond(HttpStatusCode.Accepted);
        IPetFinderTokenClient petFinderTokenClient = new PetFinderTokenClient(CreateHttpClient(), _credentials);

        // Action + Assert
        await Assert.ThrowsAsync<JsonException>(async () =>
            await petFinderTokenClient.TryGetTokenFromApiAsync(CancellationToken.None));
    }

    private HttpClient CreateHttpClient()
    {
        var httpClient = _mockHttp.ToHttpClient();
        httpClient.BaseAddress = _baseUri;
        return httpClient;
    }
}