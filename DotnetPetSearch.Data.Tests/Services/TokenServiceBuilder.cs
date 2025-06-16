using System.Net;
using System.Net.Http.Json;
using DotnetPetSearch.Data.Configurations;
using DotnetPetSearch.Data.Entities;
using DotnetPetSearch.Data.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using NSubstitute;
using RichardSzalay.MockHttp;

namespace DotnetPetSearch.Data.Tests.Services;

public class TokenServiceBuilder
{
    private Uri BaseUri { get; }
    public readonly MockHttpMessageHandler MockHttp;
    public readonly IMemoryCache MockMemoryCache;
    public readonly PetSearchContext Context;
    private readonly IOptions<PetFinderCredentials> _credentialsOptions;
    private readonly ICacheEntry _mockCacheEntry;
    private MockedRequest _mockedRequest;


    public TokenServiceBuilder(Uri baseUri, IOptions<PetFinderCredentials> credentialsOptions, DbContextOptions<PetSearchContext> dbContextOptions)
    {
        BaseUri = baseUri;
        _credentialsOptions = credentialsOptions;
        Context = new PetSearchContext(dbContextOptions);
        MockHttp = new MockHttpMessageHandler();
        _mockedRequest = new MockedRequest();
        MockMemoryCache = Substitute.For<IMemoryCache>();
        _mockCacheEntry = Substitute.For<ICacheEntry>();
    }

    public TokenService Build()
    {
        HttpClient httpClient = MockHttp.ToHttpClient();
        httpClient.BaseAddress = BaseUri;
        return new TokenService(_credentialsOptions, MockMemoryCache, Context, httpClient);
    }
    
    public async Task CreateDatabase(PetFinderToken? tokenInDatabase)
    {
        await Context.Database.EnsureDeletedAsync();
        await Context.Database.EnsureCreatedAsync();
        if (tokenInDatabase is null) return;
        await Context.Tokens.AddAsync(tokenInDatabase);
        await Context.SaveChangesAsync();
    }

    public TokenServiceBuilder CreateCache(PetFinderToken? tokenInCache)
    {
        MockMemoryCache.TryGetValue(Arg.Any<string>(), out Arg.Any<object?>())
            .Returns(callInfo =>
            {
                callInfo[1] = tokenInCache;
                return tokenInCache != null;
            });
        MockMemoryCache.CreateEntry(Arg.Any<string>()).Returns(_mockCacheEntry);
        return this;
    }

    public TokenServiceBuilder CreateServer(HttpStatusCode responseCode, string? accessTokenValue)
    {
        var url = BaseUri.ToString();
        if (responseCode == HttpStatusCode.Accepted)
        {
            if (accessTokenValue != null)
                _mockedRequest = MockHttp
                    .When(url)
                    .Respond(HttpStatusCode.Accepted, JsonContent.Create(
                        new
                        {
                            token_type = "Bearer",
                            expires_in = 3600,
                            access_token = accessTokenValue
                        }));
            else
                _mockedRequest = MockHttp
                    .When(url)
                    .Respond(HttpStatusCode.Accepted);
        }
        else
        {
            _mockedRequest = MockHttp
                .When(url)
                .Respond(responseCode);
        }
        return this;
    }

    public TokenServiceBuilder ExpectPostRequestBody<T>(T requestBody)
    {
        _mockedRequest = _mockedRequest.WithJsonContent(requestBody);
        return this;
    }
}