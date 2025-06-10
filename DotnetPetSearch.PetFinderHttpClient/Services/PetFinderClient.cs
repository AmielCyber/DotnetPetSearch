using System.Net.Http.Headers;
using System.Net.Http.Json;
using DotnetPetSearch.Data.Entities;
using DotnetPetSearch.PetFinderHttpClient.Configurations;
using DotnetPetSearch.PetFinderHttpClient.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;

namespace DotnetPetSearch.PetFinderHttpClient.Services;

/// <summary>
///     PetFinder Client implementation to call make PetFinder API calls.
/// </summary>
public class PetFinderClient : IPetFinderClient
{
    private readonly ICacheTokenService _cacheTokenService;
    private readonly HttpClient _httpClient;
    private readonly PetFinderOptions _options;
    private readonly PetFinderTokenRequest _petFinderTokenRequest;

    public PetFinderClient(IOptions<PetFinderOptions> options, IOptions<PetFinderCredentials> credentials,
        HttpClient httpClient,
        ICacheTokenService cacheTokenService)
    {
        _options = options.Value;
        _httpClient = httpClient;
        _cacheTokenService = cacheTokenService;
        _petFinderTokenRequest =
            new PetFinderTokenRequest(credentials.Value.ClientId, credentials.Value.ClientSecret);
    }

    /// <summary>
    ///     Gets a list of available pets along with its pagination data based on the passed query parameters.
    /// </summary>
    /// <param name="petsSearchParameters">Query parameters for the pet search.</param>
    /// <returns>PetFinder Pet List response which contains a list of pets and pagination metadata.</returns>
    /// <exception cref="HttpRequestException">Throws if method can not make a successful call with PetFinder.</exception>
    public async Task<PetFinderPetListResponse> GetPetsAsync(PetsSearchParameters petsSearchParameters)
    {
        await SetAuthenticationHeadersForPetFinder();
        using HttpResponseMessage response = await _httpClient.GetAsync(GetPathWithQueryString(petsSearchParameters));
        response.EnsureSuccessStatusCode();

        var petList = await response.Content.ReadFromJsonAsync<PetFinderPetListResponse>();
        if (petList is null)
            throw new HttpRequestException("Pet list could not be retrieved from Pet Finder API.");

        return petList;
    }

    /// <summary>
    ///     Retrieves a pet object containing attributes and adoption details.
    /// </summary>
    /// <param name="petId">Unique pet id to search for.</param>
    /// <returns>PetFinder Pet Response object if pet is found, else returns null.</returns>
    /// <exception cref="HttpRequestException">Throws if method can not make a successful call with PetFinder.</exception>
    public async Task<PetFinderPetResponse?> GetSinglePetByIdAsync(int petId)
    {
        await SetAuthenticationHeadersForPetFinder();
        using HttpResponseMessage response = await _httpClient.GetAsync($"{_options.PetSearchUrl}/{petId}");
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<PetFinderPetResponse>();
    }

    private string GetPathWithQueryString(PetsSearchParameters petsSearchParameters)
    {
        List<KeyValuePair<string, string?>> query =
        [
            new("type", petsSearchParameters.Type),
            new("location", petsSearchParameters.Location),
            new("page", petsSearchParameters.Page.ToString()),
            new("distance", petsSearchParameters.Distance.ToString()),
            new("sort", petsSearchParameters.Sort)
        ];
        return QueryHelpers.AddQueryString(_options.PetSearchUrl, query);
    }

    private async Task SetAuthenticationHeadersForPetFinder()
    {
        PetFinderToken token = await GetToken();
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, token.AccessToken);
    }

    private async Task<PetFinderToken> GetToken()
    {
        PetFinderToken? token = await _cacheTokenService.GetToken();
        if (token is not null) return token;

        token = await GetNewToken();
        await StoreToken(token);
        return token;
    }

    private async Task<PetFinderToken> GetNewToken()
    {
        DateTime expiresIn = DateTime.Now.AddMinutes(60);

        using HttpResponseMessage response =
            await _httpClient.PostAsJsonAsync(_options.TokenUrl, _petFinderTokenRequest);
        response.EnsureSuccessStatusCode();
        var petFinderToken = await response.Content.ReadFromJsonAsync<TokenResponse>();
        if (petFinderToken is null)
            throw new HttpRequestException("Failed to obtain pet finder token!");

        return new PetFinderToken
        {
            AccessToken = petFinderToken.AccessToken,
            ExpiresIn = expiresIn
        };
    }

    private async Task StoreToken(PetFinderToken token)
    {
        _cacheTokenService.StoreTokenInCache(token);
        await _cacheTokenService.StoreTokenInDatabase(token);
    }
}