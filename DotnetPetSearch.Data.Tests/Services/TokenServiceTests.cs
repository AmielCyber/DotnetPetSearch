using System.Net;
using DotnetPetSearch.Data.Configurations;
using DotnetPetSearch.Data.Entities;
using DotnetPetSearch.Data.Services;
using DotnetPetSearch.Data.Tests.Fixtures;
using DotnetPetSearch.Data.Tests.TestData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using NSubstitute;
using RichardSzalay.MockHttp;

namespace DotnetPetSearch.Data.Tests.Services;

public class TokenServiceTests : IClassFixture<TokenServiceFixture>
{
    private readonly PetFinderToken _expiredPetFinderToken;
    private readonly TokenServiceFixture _tokenServiceFixture;
    private readonly PetFinderToken _validPetFinderToken;

    public TokenServiceTests(TokenServiceFixture tokenServiceFixture)
    {
        _tokenServiceFixture = tokenServiceFixture;
        _validPetFinderToken = new PetFinderToken { AccessToken = string.Empty };
        _expiredPetFinderToken = new PetFinderToken
        {
            ExpiresIn = DateTime.Now.AddSeconds(-1),
            AccessToken = string.Empty
        };
    }

    [Theory]
    [ClassData(typeof(ValidPetFinderTokens))]
    public async Task GetTokenAsync_ShouldReturnTokenFromCache_WhenTokenInCacheIsValid(PetFinderToken validToken)
    {
        // Arrange
        IOptions<PetFinderCredentials> credentials = _tokenServiceFixture.CreateCredentials();
        IMemoryCache memoryCache = _tokenServiceFixture.CreatMemoryCache(validToken);
        PetSearchContext petSearchContext = await _tokenServiceFixture.CreateDatabase(null);
        MockHttpMessageHandler mockHttpMessage = _tokenServiceFixture.CreateServer(null, HttpStatusCode.Accepted);
        HttpClient client = CreateHttpClient(mockHttpMessage);


        var tokenService = new TokenService(credentials, memoryCache, petSearchContext, client);

        // Action
        PetFinderToken token = await tokenService.GetTokenAsync();

        // Assert
        Assert.Equal(token.AccessToken, _tokenServiceFixture.TokenFromCache);
        memoryCache.ReceivedWithAnyArgs().TryGetValue(Arg.Any<string>(), out Arg.Any<object?>());
        mockHttpMessage.VerifyNoOutstandingRequest();
    }

    [Theory]
    [ClassData(typeof(ExpiredPetFinderTokens))]
    public async Task GetTokenAsync_ShouldReturnTokenFromDataBase_WhenTokenInDbIsValid_AndTokenInCacheIsExpired(
        PetFinderToken expiredToken)
    {
        // Arrange
        IOptions<PetFinderCredentials> credentials = _tokenServiceFixture.CreateCredentials();
        IMemoryCache memoryCache = _tokenServiceFixture.CreatMemoryCache(expiredToken);
        PetSearchContext petSearchContext = await _tokenServiceFixture.CreateDatabase(_validPetFinderToken);
        MockHttpMessageHandler mockHttpMessage = _tokenServiceFixture.CreateServer(null, HttpStatusCode.Accepted);
        HttpClient client = CreateHttpClient(mockHttpMessage);
        var tokenService = new TokenService(credentials, memoryCache, petSearchContext, client);

        // Action
        PetFinderToken token = await tokenService.GetTokenAsync();

        // Assert
        Assert.Equal(token.AccessToken, _tokenServiceFixture.TokenFromDatabase);
        memoryCache.ReceivedWithAnyArgs().TryGetValue(Arg.Any<string>(), out Arg.Any<object?>());
        mockHttpMessage.VerifyNoOutstandingRequest();
    }

    [Fact]
    public async Task GetTokenAsync_ShouldReturnTokenFromDataBase_WhenTokenInDbIsValid_AndTokenInCacheHasNoToken()
    {
        // Arrange
        IOptions<PetFinderCredentials> credentials = _tokenServiceFixture.CreateCredentials();
        IMemoryCache memoryCache = _tokenServiceFixture.CreatMemoryCache(null);
        PetSearchContext petSearchContext = await _tokenServiceFixture.CreateDatabase(_validPetFinderToken);
        MockHttpMessageHandler mockHttpMessage = _tokenServiceFixture.CreateServer(null, HttpStatusCode.Accepted);
        HttpClient client = CreateHttpClient(mockHttpMessage);
        var tokenService = new TokenService(credentials, memoryCache, petSearchContext, client);

        // Action
        PetFinderToken token = await tokenService.GetTokenAsync();

        // Assert
        Assert.Equal(token.AccessToken, _tokenServiceFixture.TokenFromDatabase);
        memoryCache.ReceivedWithAnyArgs().TryGetValue(Arg.Any<string>(), out Arg.Any<object?>());
        mockHttpMessage.VerifyNoOutstandingRequest();
    }

    [Fact]
    public async Task GetTokenAsync_ShouldSaveTokenInCache_WhenGettingTokenFromDatabase()
    {
        // Arrange
        IOptions<PetFinderCredentials> credentials = _tokenServiceFixture.CreateCredentials();
        IMemoryCache memoryCache = _tokenServiceFixture.CreatMemoryCache(null);
        PetSearchContext petSearchContext = await _tokenServiceFixture.CreateDatabase(_validPetFinderToken);
        MockHttpMessageHandler mockHttpMessage = _tokenServiceFixture.CreateServer(null, HttpStatusCode.Accepted);
        HttpClient client = CreateHttpClient(mockHttpMessage);
        var tokenService = new TokenService(credentials, memoryCache, petSearchContext, client);

        // Action
        PetFinderToken result = await tokenService.GetTokenAsync();

        Assert.Equal(_tokenServiceFixture.TokenFromDatabase, result.AccessToken);
        memoryCache.ReceivedWithAnyArgs().TryGetValue(Arg.Any<string>(), out Arg.Any<object?>());
        memoryCache.ReceivedWithAnyArgs().Set(Arg.Any<string>(), result);
        mockHttpMessage.VerifyNoOutstandingRequest();
    }

    [Fact]
    public async Task GetTokenAsync_ShouldGetTokenFromServer_WhenTokenInDbAndInCacheDontHaveToken()
    {
        // Arrange
        IOptions<PetFinderCredentials> credentials = _tokenServiceFixture.CreateCredentials();
        IMemoryCache memoryCache = _tokenServiceFixture.CreatMemoryCache(null);
        PetSearchContext petSearchContext = await _tokenServiceFixture.CreateDatabase(null);
        MockHttpMessageHandler mockHttpMessage =
            _tokenServiceFixture.CreateServer(_validPetFinderToken, HttpStatusCode.Accepted);
        HttpClient client = CreateHttpClient(mockHttpMessage);
        var tokenService = new TokenService(credentials, memoryCache, petSearchContext, client);

        // Action
        PetFinderToken result = await tokenService.GetTokenAsync();

        // Assert
        Assert.Equal(_tokenServiceFixture.TokenFromServer, result.AccessToken);
        memoryCache.ReceivedWithAnyArgs().TryGetValue(Arg.Any<string>(), out Arg.Any<object?>());
        mockHttpMessage.Expect(_tokenServiceFixture.BaseUri.ToString()).Respond(HttpStatusCode.Accepted);
    }

    [Fact]
    public async Task GetTokenAsync_ShouldGetTokenFromServer_WhenTokenInDbAndInCacheHaveExpiredToken()
    {
        // Arrange
        IOptions<PetFinderCredentials> credentials = _tokenServiceFixture.CreateCredentials();
        IMemoryCache memoryCache = _tokenServiceFixture.CreatMemoryCache(_expiredPetFinderToken);
        PetSearchContext petSearchContext = await _tokenServiceFixture.CreateDatabase(_expiredPetFinderToken);
        MockHttpMessageHandler mockHttpMessage =
            _tokenServiceFixture.CreateServer(_validPetFinderToken, HttpStatusCode.Accepted);
        HttpClient client = CreateHttpClient(mockHttpMessage);
        var tokenService = new TokenService(credentials, memoryCache, petSearchContext, client);

        // Action
        PetFinderToken result = await tokenService.GetTokenAsync();

        // Assert
        Assert.Equal(_tokenServiceFixture.TokenFromServer, result.AccessToken);
        memoryCache.ReceivedWithAnyArgs().TryGetValue(Arg.Any<string>(), out Arg.Any<object?>());
        mockHttpMessage.Expect(_tokenServiceFixture.BaseUri.ToString()).Respond(HttpStatusCode.Accepted);
    }

    [Fact]
    public async Task GetTokenAsync_ShouldGetTokenFromServer_WhenTokenInDbAndInCacheDontHaveAToken()
    {
        // Arrange
        IOptions<PetFinderCredentials> credentials = _tokenServiceFixture.CreateCredentials();
        IMemoryCache memoryCache = _tokenServiceFixture.CreatMemoryCache(null);
        PetSearchContext petSearchContext = await _tokenServiceFixture.CreateDatabase(null);
        MockHttpMessageHandler mockHttpMessage =
            _tokenServiceFixture.CreateServer(_validPetFinderToken, HttpStatusCode.Accepted);
        HttpClient client = CreateHttpClient(mockHttpMessage);
        var tokenService = new TokenService(credentials, memoryCache, petSearchContext, client);

        // Action
        PetFinderToken result = await tokenService.GetTokenAsync();

        // Assert
        Assert.Equal(_tokenServiceFixture.TokenFromServer, result.AccessToken);
        memoryCache.ReceivedWithAnyArgs().TryGetValue(Arg.Any<string>(), out Arg.Any<object?>());
        mockHttpMessage.Expect(_tokenServiceFixture.BaseUri.ToString()).Respond(HttpStatusCode.Accepted);
    }

    [Fact]
    public async Task GetTokenAsync_ShouldSaveTokenInDatabase_WhenGettingTokenFromServer()
    {
        // Arrange
        IOptions<PetFinderCredentials> credentials = _tokenServiceFixture.CreateCredentials();
        IMemoryCache memoryCache = _tokenServiceFixture.CreatMemoryCache(null);
        PetSearchContext petSearchContext = await _tokenServiceFixture.CreateDatabase(null);
        MockHttpMessageHandler mockHttpMessage =
            _tokenServiceFixture.CreateServer(_validPetFinderToken, HttpStatusCode.Accepted);
        HttpClient client = CreateHttpClient(mockHttpMessage);
        var tokenService = new TokenService(credentials, memoryCache, petSearchContext, client);

        // Action
        PetFinderToken result = await tokenService.GetTokenAsync();

        petSearchContext.ChangeTracker.Clear();
        PetFinderToken? dbToken = await petSearchContext.Set<PetFinderToken>()
            .SingleOrDefaultAsync(t => t.Id == _validPetFinderToken.Id);

        Assert.Equal(_tokenServiceFixture.TokenFromServer, result.AccessToken);
        Assert.Equal(result.AccessToken, dbToken?.AccessToken);
        memoryCache.ReceivedWithAnyArgs().TryGetValue(Arg.Any<string>(), out Arg.Any<object?>());
        memoryCache.ReceivedWithAnyArgs().Set(Arg.Any<string>(), result);
        mockHttpMessage.Expect(_tokenServiceFixture.BaseUri.ToString()).Respond(HttpStatusCode.Accepted);
    }

    [Fact]
    public async Task GetTokenAsync_ShouldSaveTokenInCache_WhenGettingTokenFromServer()
    {
        // Arrange
        IOptions<PetFinderCredentials> credentials = _tokenServiceFixture.CreateCredentials();
        IMemoryCache memoryCache = _tokenServiceFixture.CreatMemoryCache(null);
        PetSearchContext petSearchContext = await _tokenServiceFixture.CreateDatabase(null);
        MockHttpMessageHandler mockHttpMessage =
            _tokenServiceFixture.CreateServer(_validPetFinderToken, HttpStatusCode.Accepted);
        HttpClient client = CreateHttpClient(mockHttpMessage);
        var tokenService = new TokenService(credentials, memoryCache, petSearchContext, client);

        // Action
        PetFinderToken result = await tokenService.GetTokenAsync();

        // Assert
        Assert.Equal(_tokenServiceFixture.TokenFromServer, result.AccessToken);
        memoryCache.ReceivedWithAnyArgs().TryGetValue(Arg.Any<string>(), out Arg.Any<object?>());
        memoryCache.ReceivedWithAnyArgs().Set(Arg.Any<string>(), result);
        mockHttpMessage.VerifyNoOutstandingRequest();
    }

    private HttpClient CreateHttpClient(MockHttpMessageHandler mockHttpMessageHandler)
    {
        var httpClient = mockHttpMessageHandler.ToHttpClient();
        httpClient.BaseAddress = _tokenServiceFixture.BaseUri;
        return httpClient;
    }
}