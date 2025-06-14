using System.Data.Common;
using System.Net;
using System.Net.Http.Json;
using DotnetPetSearch.Data.Configurations;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using NSubstitute;
using RichardSzalay.MockHttp;
using PetFinderToken = DotnetPetSearch.Data.Entities.PetFinderToken;

namespace DotnetPetSearch.Data.Tests.Fixtures;

// TODO: Create DI fixture
public class TokenServiceFixture : IDisposable
{
    private readonly DbConnection _dbConnection;

    private readonly DbContextOptions<PetSearchContext> _dbContextOptions;
    public readonly Uri BaseUri;
    public readonly string TokenFromCache = "cache";
    public readonly string TokenFromDatabase = "database";
    public readonly string TokenFromServer = "server";

    public TokenServiceFixture()
    {
        BaseUri = new Uri("http://petfinder.com/v2/token");
        _dbConnection = new SqliteConnection("Filename=:memory:");
        _dbConnection.Open();
        _dbContextOptions = new DbContextOptionsBuilder<PetSearchContext>()
            .UseSqlite(_dbConnection)
            .Options;
    }

    public void Dispose()
    {
        _dbConnection.Dispose();
    }

    public IMemoryCache CreatMemoryCache(PetFinderToken? tokenInCache)
    {
        var memoryCache = Substitute.For<IMemoryCache>();
        memoryCache.ClearReceivedCalls();
        if (tokenInCache != null)
            tokenInCache = new PetFinderToken
            {
                ExpiresIn = tokenInCache.ExpiresIn,
                AccessToken = TokenFromCache
            };
        memoryCache.TryGetValue(Arg.Any<string>(), out Arg.Any<object?>())
            .Returns(callInfo =>
            {
                callInfo[1] = tokenInCache;
                return tokenInCache != null;
            });
        return memoryCache;
    }

    public async Task<PetSearchContext> CreateDatabase(PetFinderToken? tokenInDatabase)
    {
        var petSearchContext = new PetSearchContext(_dbContextOptions);
        await petSearchContext.Database.EnsureDeletedAsync();
        await petSearchContext.Database.EnsureCreatedAsync();
        if (tokenInDatabase is not null)
        {
            tokenInDatabase = new PetFinderToken
            {
                ExpiresIn = tokenInDatabase.ExpiresIn,
                AccessToken = TokenFromDatabase
            };
            await petSearchContext.Tokens.AddAsync(tokenInDatabase);
            await petSearchContext.SaveChangesAsync();
        }

        return petSearchContext;
    }

    public MockHttpMessageHandler CreateServer(PetFinderToken? tokenInServer, HttpStatusCode serverResponseCode)
    {
        var mockHttpMessageHandler = new MockHttpMessageHandler();
        var url = BaseUri.ToString();
        if (serverResponseCode == HttpStatusCode.Accepted)
        {
            if (tokenInServer != null)
                mockHttpMessageHandler
                    .When(url)
                    .Respond(HttpStatusCode.Accepted, JsonContent.Create(
                        new
                        {
                            token_type = "Bearer",
                            expires_in = 3600,
                            access_token = TokenFromServer
                        }));
            else
                mockHttpMessageHandler
                    .When(url)
                    .Respond(HttpStatusCode.Accepted);
        }
        else
        {
            mockHttpMessageHandler
                .When(url)
                .Respond(serverResponseCode);
        }

        return mockHttpMessageHandler;
    }

    public IOptions<PetFinderCredentials> CreateCredentials()
    {
        return Options.Create(new PetFinderCredentials
        {
            ClientId = "client",
            ClientSecret = "secret"
        });
    }
}