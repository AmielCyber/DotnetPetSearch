using System.Net;
using DotnetPetSearch.Data.Entities;
using DotnetPetSearch.Data.Services;
using DotnetPetSearch.Data.Tests.Fixtures;
using DotnetPetSearch.Data.Tests.TestData;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using static NSubstitute.Arg;

namespace DotnetPetSearch.Data.Tests.Services;

public class TokenServiceTests : IClassFixture<TokenServiceFixture>
{
    private readonly TokenServiceFixture _tokenServiceFixture;
    private readonly TokenServiceBuilder _tokenServiceBuilder;

    public TokenServiceTests(TokenServiceFixture tokenServiceFixture)
    {
        _tokenServiceFixture = tokenServiceFixture;
        _tokenServiceBuilder = new TokenServiceBuilder(_tokenServiceFixture.BaseUri,
            _tokenServiceFixture.ExpectedCredentialOptions, _tokenServiceFixture.DbContextOptions);
    }

    [Theory]
    [ClassData(typeof(FutureDateTime))]
    public async Task GetTokenAsync_ShouldReturnTokenFromCache_WhenTokenInCacheIsValid(DateTime futureDateTime)
    {
        // Arrange
        PetFinderToken tokenInCache = new PetFinderToken()
        {
            ExpiresIn = futureDateTime, AccessToken = _tokenServiceFixture.TokenFromCache,
        };
        await _tokenServiceBuilder.CreateDatabase(null);
        TokenService tokenService = _tokenServiceBuilder
            .CreateCache(tokenInCache)
            .CreateServer(HttpStatusCode.Accepted, null)
            .Build();

        // Action
        PetFinderToken token = await tokenService.GetTokenAsync();

        // Assert
        Assert.Equal(token.AccessToken, _tokenServiceFixture.TokenFromCache);
        _tokenServiceBuilder.MockMemoryCache.ReceivedWithAnyArgs().TryGetValue(Any<string>(), out Any<object?>());
        _tokenServiceBuilder.MockHttp.VerifyNoOutstandingRequest();
    }

    [Theory]
    [ClassData(typeof(PastDateTime))]
    public async Task GetTokenAsync_ShouldReturnTokenFromDataBase_WhenTokenInDbIsValid_AndTokenInCacheIsExpired(
        DateTime pastDateTime)
    {
        // Arrange
        PetFinderToken tokenInCache = new PetFinderToken()
        {
            ExpiresIn = pastDateTime, AccessToken = _tokenServiceFixture.TokenFromCache,
        };
        PetFinderToken tokenInDataBase = new PetFinderToken() { AccessToken = _tokenServiceFixture.TokenFromDatabase };
        
        await _tokenServiceBuilder.CreateDatabase(tokenInDataBase);
        TokenService tokenService = _tokenServiceBuilder
            .CreateCache(tokenInCache)
            .CreateServer(HttpStatusCode.Accepted, null)
            .Build();

        // Action
        PetFinderToken token = await tokenService.GetTokenAsync();

        // Assert
        Assert.Equal(token.AccessToken, _tokenServiceFixture.TokenFromDatabase);
        _tokenServiceBuilder.MockMemoryCache.ReceivedWithAnyArgs().TryGetValue(Any<string>(), out Any<object?>());
        _tokenServiceBuilder.MockHttp.VerifyNoOutstandingRequest();
    }

    [Fact]
    public async Task GetTokenAsync_ShouldReturnTokenFromDataBase_WhenTokenInDbIsValid_AndTokenInCacheHasNoToken()
    {
        // Arrange
        PetFinderToken tokenInDataBase = new PetFinderToken() { AccessToken = _tokenServiceFixture.TokenFromDatabase };
        await _tokenServiceBuilder.CreateDatabase(tokenInDataBase);
        TokenService tokenService = _tokenServiceBuilder
            .CreateCache(null)
            .CreateServer(HttpStatusCode.Accepted, null)
            .Build();

        // Action
        PetFinderToken token = await tokenService.GetTokenAsync();

        // Assert
        Assert.Equal(token.AccessToken, _tokenServiceFixture.TokenFromDatabase);
        _tokenServiceBuilder.MockMemoryCache.ReceivedWithAnyArgs().TryGetValue(Any<string>(), out Any<object?>());
        _tokenServiceBuilder.MockHttp.VerifyNoOutstandingRequest();
    }

    [Fact]
    public async Task GetTokenAsync_ShouldSaveTokenInCache_WhenGettingTokenFromDatabase()
    {
        // Arrange
        PetFinderToken tokenInDataBase = new PetFinderToken() { AccessToken = _tokenServiceFixture.TokenFromDatabase };
        await _tokenServiceBuilder.CreateDatabase(tokenInDataBase);
        TokenService tokenService = _tokenServiceBuilder
            .CreateCache(null)
            .CreateServer(HttpStatusCode.Accepted, null)
            .Build();
        
        // Action
        PetFinderToken result = await tokenService.GetTokenAsync();

        Assert.Equal(_tokenServiceFixture.TokenFromDatabase, result.AccessToken);
        _tokenServiceBuilder.MockMemoryCache.ReceivedWithAnyArgs().TryGetValue(Any<string>(), out Any<object?>());
        _tokenServiceBuilder.MockMemoryCache.ReceivedWithAnyArgs().CreateEntry(Any<string>());
        _tokenServiceBuilder.MockHttp.VerifyNoOutstandingRequest();
    }

    [Fact]
    public async Task GetTokenAsync_ShouldGetTokenFromServer_WhenTokenInDbAndInCacheDontHaveToken()
    {
        // Arrange
        await _tokenServiceBuilder.CreateDatabase(null);
        TokenService tokenService = _tokenServiceBuilder
            .CreateCache(null)
            .CreateServer(HttpStatusCode.Accepted, _tokenServiceFixture.TokenFromServer)
            .Build();
        
        // Action
        PetFinderToken result = await tokenService.GetTokenAsync();

        // Assert
        Assert.Equal(_tokenServiceFixture.TokenFromServer, result.AccessToken);
        _tokenServiceBuilder.MockMemoryCache.ReceivedWithAnyArgs().TryGetValue(Arg.Any<string>(), out Arg.Any<object?>());
        _tokenServiceBuilder.MockHttp.VerifyNoOutstandingRequest();
    }
    
    [Fact]
    public async Task GetTokenAsync_ShouldPost_WithCredentialBodyContent_WhenGettingTokenFromServer()
    {
        // Arrange
        await _tokenServiceBuilder.CreateDatabase(null);
        TokenService tokenService = _tokenServiceBuilder
            .CreateCache(null)
            .CreateServer(HttpStatusCode.Accepted, _tokenServiceFixture.TokenFromServer)
            .ExpectPostRequestBody(_tokenServiceFixture.ExpectedTokenRequestBody)
            .Build();
        
        // Action
        PetFinderToken result = await tokenService.GetTokenAsync();

        // Assert
        Assert.Equal(_tokenServiceFixture.TokenFromServer, result.AccessToken);
        _tokenServiceBuilder.MockHttp.VerifyNoOutstandingRequest();
    }

    [Theory]
    [ClassData(typeof(PastDateTime))]
    public async Task GetTokenAsync_ShouldGetTokenFromServer_WhenTokenInDbAndInCacheHaveExpiredToken(DateTime pastDateTime)
    {
        // Arrange
        PetFinderToken tokenInCache = new PetFinderToken()
        {
            ExpiresIn = pastDateTime, AccessToken = _tokenServiceFixture.TokenFromCache,
        };
        PetFinderToken tokenInDataBase = new PetFinderToken()
        {
            ExpiresIn = pastDateTime,
            AccessToken = _tokenServiceFixture.TokenFromDatabase
        };
        
        await _tokenServiceBuilder.CreateDatabase(tokenInDataBase);
        TokenService tokenService = _tokenServiceBuilder
            .CreateCache(tokenInCache)
            .CreateServer(HttpStatusCode.Accepted, _tokenServiceFixture.TokenFromServer)
            .Build();

        // Action
        PetFinderToken result = await tokenService.GetTokenAsync();

        // Assert
        Assert.Equal(_tokenServiceFixture.TokenFromServer, result.AccessToken);
        _tokenServiceBuilder.MockMemoryCache.ReceivedWithAnyArgs().TryGetValue(Arg.Any<string>(), out Arg.Any<object?>());
        _tokenServiceBuilder.MockHttp.VerifyNoOutstandingRequest();
    }

    [Fact]
    public async Task GetTokenAsync_ShouldSaveTokenInDatabase_WhenGettingTokenFromServer()
    {
        // Arrange
        await _tokenServiceBuilder.CreateDatabase(null);
        TokenService tokenService = _tokenServiceBuilder
            .CreateCache(null)
            .CreateServer(HttpStatusCode.Accepted, _tokenServiceFixture.TokenFromServer)
            .Build();
        
        // Action
        PetFinderToken result = await tokenService.GetTokenAsync();

        // Assert
        PetFinderToken? dbToken = await _tokenServiceBuilder.Context.Set<PetFinderToken>()
            .SingleOrDefaultAsync();
        
        Assert.Equal(_tokenServiceFixture.TokenFromServer, result.AccessToken);
        Assert.Equal(result.AccessToken, dbToken?.AccessToken);
        _tokenServiceBuilder.MockMemoryCache.ReceivedWithAnyArgs().TryGetValue(Arg.Any<string>(), out Arg.Any<object?>());
        _tokenServiceBuilder.MockMemoryCache.ReceivedWithAnyArgs().CreateEntry(Any<string>());
        _tokenServiceBuilder.MockHttp.VerifyNoOutstandingRequest();
    }

    [Fact]
    public async Task GetTokenAsync_ShouldSaveTokenInCache_WhenGettingTokenFromServer()
    {
        // Arrange
        await _tokenServiceBuilder.CreateDatabase(null);
        TokenService tokenService = _tokenServiceBuilder
            .CreateCache(null)
            .CreateServer(HttpStatusCode.Accepted, _tokenServiceFixture.TokenFromServer)
            .Build();
        
        // Action
        PetFinderToken result = await tokenService.GetTokenAsync();

        // Assert
        Assert.Equal(_tokenServiceFixture.TokenFromServer, result.AccessToken);
        _tokenServiceBuilder.MockMemoryCache.ReceivedWithAnyArgs().TryGetValue(Arg.Any<string>(), out Arg.Any<object?>());
        _tokenServiceBuilder.MockMemoryCache.ReceivedWithAnyArgs().CreateEntry(Any<string>());
        _tokenServiceBuilder.MockHttp.VerifyNoOutstandingRequest();
    }
}