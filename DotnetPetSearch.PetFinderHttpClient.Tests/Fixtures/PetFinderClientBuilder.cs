using System.Net;
using System.Net.Http.Json;
using DotnetPetSearch.Data.Entities;
using DotnetPetSearch.PetFinderHttpClient.Services;
using NSubstitute;
using NSubstitute.ClearExtensions;
using NSubstitute.ReturnsExtensions;
using RichardSzalay.MockHttp;

namespace DotnetPetSearch.PetFinderHttpClient.Tests.Fixtures;

public class PetFinderClientBuilder
{
    public MockHttpMessageHandler MockHttp { get; }


    private ITokenCacheService TokenCacheService { get; }
    private readonly Uri _baseUri;
    private MockedRequest MockedRequest { get; set; }

    public PetFinderClientBuilder(Uri baseUri, PetFinderToken expectedToken)
    {
        _baseUri = baseUri;
        MockHttp = new MockHttpMessageHandler();
        TokenCacheService = Substitute.For<ITokenCacheService>();
        TokenCacheService.TryGetToken().Returns(expectedToken);
        MockedRequest = new MockedRequest();
    }

    public PetFinderClient Build()
    {
        var httpClient = MockHttp.ToHttpClient();
        httpClient.BaseAddress = _baseUri;
        return new PetFinderClient(httpClient, TokenCacheService);
    }

    public PetFinderClientBuilder SetExpectHttpRequestUrl(string url)
    {
        MockHttp.Clear();
        MockedRequest = MockHttp.When($"{_baseUri.ToString()}{url}");
        return this;
    }

    public PetFinderClientBuilder SetExpectHttpRequestHeader(string headerName, string headerValue)
    {
        MockedRequest = MockedRequest.WithHeaders(headerName, headerValue);
        return this;
    }

    public PetFinderClientBuilder SetExpectHttpQueryRequest(Dictionary<string, string> queryParameters)
    {
        MockedRequest = MockedRequest.WithExactQueryString(queryParameters);
        return this;
    }

    public PetFinderClientBuilder SetExpectHttpResponse(HttpStatusCode statusCode, object? content)
    {
        content ??= new { };
        MockedRequest = MockedRequest.Respond(statusCode, JsonContent.Create(content));
        return this;
    }

    public PetFinderClientBuilder SetEmptyTokenInCache()
    {
        TokenCacheService.ClearSubstitute();
        TokenCacheService.TryGetToken().ReturnsNull();
        return this;
    }
}