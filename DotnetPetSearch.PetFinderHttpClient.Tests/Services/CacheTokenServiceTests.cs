using System.Data.Common;
using DotnetPetSearch.Data;
using DotnetPetSearch.Data.Entities;
using DotnetPetSearch.PetFinderHttpClient.Services;
using DotnetPetSearch.PetFinderHttpClient.Tests.TestData;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using NSubstitute;

namespace DotnetPetSearch.PetFinderHttpClient.Tests.Services;

public class CacheTokenServiceTests : IDisposable
{
    private readonly PetFinderToken _expectedPetFinderToken;
    private readonly IMemoryCache _memoryCache;
    private readonly DbConnection _dbConnection;


    public CacheTokenServiceTests()
    {
        _expectedPetFinderToken = new PetFinderToken { AccessToken = Guid.NewGuid().ToString() };
        _memoryCache = NSubstitute.Substitute.For<IMemoryCache>();

        _dbConnection = new SqliteConnection("Filename=:memory:");
        _dbConnection.Open();
    }

    [Theory]
    [ClassData(typeof(ExpiredPetFinderTokens))]
    public async Task IsTokenExpired_TokenExpired_ReturnsTrue_WhenTokenExpires(PetFinderToken expiredPetFinderToken)
    {
        PetSearchContext petSearchContext = await CreatePetSearchContext();
        var cacheTokenService = new CacheTokenService(_memoryCache, petSearchContext);
        var result = cacheTokenService.IsTokenExpired(expiredPetFinderToken);
        Assert.True(result);
    }
    
    [Theory]
    [ClassData(typeof(ValidPetFinderTokens))]
    public async Task IsTokenExpired_TokenExpired_ReturnsFalse_WhenTokenIsNotExpired(PetFinderToken expiredPetFinderToken)
    {
        PetSearchContext petSearchContext = await CreatePetSearchContext();
        var cacheTokenService = new CacheTokenService(_memoryCache, petSearchContext);
        var result = cacheTokenService.IsTokenExpired(expiredPetFinderToken);
        Assert.False(result);
    }

    [Fact]
    public async Task StoreTokenInCache_ShouldSetThePassedTokenToTheCache()
    {
        PetSearchContext petSearchContext = await CreatePetSearchContext();
        
        var cacheTokenService = new CacheTokenService(_memoryCache, petSearchContext);
        // Action
        cacheTokenService.StoreTokenInCache(_expectedPetFinderToken);
        //  Assert
        _memoryCache.ReceivedWithAnyArgs().Set("", _expectedPetFinderToken, new MemoryCacheEntryOptions());
    }
    
    [Fact]
    public async Task StoreTokenInCache_ShouldStoreToken_WithTheExpirationOfTheCache()
    {
        PetSearchContext petSearchContext = await CreatePetSearchContext();
        var cacheTokenService = new CacheTokenService(_memoryCache, petSearchContext);
        // Action
        cacheTokenService.StoreTokenInCache(_expectedPetFinderToken);
        //  Assert
        // calculator.Received().Add(Arg.Any<int>(), 2);
        _memoryCache.Received()
            .Set(Arg.Any<string>(), _expectedPetFinderToken, new MemoryCacheEntryOptions().SetAbsoluteExpiration(_expectedPetFinderToken.ExpiresIn));
    }

    [Fact]
    public async Task StoreTokenInDatabase_ShouldSaveTheTokenToTheDatabase()
    {
        PetSearchContext petSearchContext = await CreatePetSearchContext();
        var cacheTokenService = new CacheTokenService(_memoryCache, petSearchContext);
        await cacheTokenService.StoreTokenInDatabaseAsync(_expectedPetFinderToken);
        int totalCount = await petSearchContext.Tokens.Where(t => t.Id == _expectedPetFinderToken.Id).CountAsync();
        Assert.Equal(1, totalCount);
    }
    
    [Fact]
    public async Task GetToken_ShouldReturnTheTokenFromTheCache_WhenCacheHasToken()
    {
        PetSearchContext petSearchContext = await CreatePetSearchContext();
        _memoryCache.TryGetValue(Arg.Any<string>(), out Arg.Any<object>())
            .Returns(callInfo =>
            {
                callInfo[1] = _expectedPetFinderToken; 
                return true; 
            });
        var cacheTokenService = new CacheTokenService(_memoryCache, petSearchContext);
        PetFinderToken? token = await cacheTokenService.GetTokenAsync();
        
        Assert.NotNull(token);
        Assert.Equal(_expectedPetFinderToken.Id, token!.Id);
    }
    [Fact]
    public async Task GetToken_ShouldReturnTheTokenFromTheDatabase_WhenDatabaseHasToken()
    {
        PetSearchContext petSearchContext = await CreatePetSearchContext(_expectedPetFinderToken);
        _memoryCache.TryGetValue(Arg.Any<string>(), out Arg.Any<object>())
            .Returns(callInfo =>
            {
                callInfo[1] = null;
                return false; 
            });
        var cacheTokenService = new CacheTokenService(_memoryCache, petSearchContext);
        PetFinderToken? token = await cacheTokenService.GetTokenAsync();

        Assert.NotNull(token);
        Assert.Equal(_expectedPetFinderToken.Id, token!.Id);
    }
    
    [Fact]
    public async Task GetToken_ShouldStoreTokenInCache_WhenDatabaseHasToken()
    {
        PetSearchContext petSearchContext = await CreatePetSearchContext(_expectedPetFinderToken);
        _memoryCache.TryGetValue(Arg.Any<string>(), out Arg.Any<object>())
            .Returns(callInfo =>
            {
                callInfo[1] = null;
                return false; 
            });
        var cacheTokenService = new CacheTokenService(_memoryCache, petSearchContext);
        PetFinderToken? token = await cacheTokenService.GetTokenAsync();

        Assert.NotNull(token);
        Assert.Equal(_expectedPetFinderToken.Id, token!.Id);
        _memoryCache.ReceivedWithAnyArgs().Set(Arg.Any<string>(), Arg.Any<PetFinderToken>);
    }
    
    private async Task<PetSearchContext> CreatePetSearchContext(PetFinderToken? dbToken = null)
    {
        DbContextOptions<PetSearchContext> options = new DbContextOptionsBuilder<PetSearchContext>()
            .UseSqlite(_dbConnection)
            .Options;

        // Create the schema and seed some data
        PetSearchContext context = new PetSearchContext(options);
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        if (dbToken is not null)
        {
            await context.Tokens.AddAsync(dbToken);
            await context.SaveChangesAsync();
        }
        return context;
    }

    public void Dispose() => _dbConnection.Dispose();
}