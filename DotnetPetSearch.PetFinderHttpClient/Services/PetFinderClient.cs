using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using DotnetPetSearch.Data.Entities;
using DotnetPetSearch.PetFinderHttpClient.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.WebUtilities;

namespace DotnetPetSearch.PetFinderHttpClient.Services;

/// <summary>
///     PetFinder Client implementation to call make PetFinder API calls.
/// </summary>
public class PetFinderClient : IPetFinderClient
{
    private readonly HttpClient _httpClient;
    private readonly ITokenCacheService _tokenCacheService;

    public PetFinderClient(HttpClient httpClient, ITokenCacheService tokenCacheService)
    {
        _httpClient = httpClient;
        _tokenCacheService = tokenCacheService;
    }

    /// <summary>
    ///     Gets a list of available pets along with its pagination data based on the passed query parameters.
    /// </summary>
    /// <param name="petSearchParameters">Query parameters for the pet search.</param>
    /// <returns>PetFinder Pet List response which contains a list of pets and pagination metadata.</returns>
    /// <exception cref="HttpRequestException">Throws if method can not make a successful call with PetFinder.</exception>
    public async Task<PetFinderPetListResponse> GetPetsAsync(IPetSearchParameters petSearchParameters)
    {
        SetAuthenticationRequestHeadersAsync();
        using HttpResponseMessage response = await _httpClient.GetAsync(GetPathWithQueryString(petSearchParameters));
        response.EnsureSuccessStatusCode();

        var petList = await response.Content.ReadFromJsonAsync<PetFinderPetListResponse>();
        if (petList?.Animals == null || petList.Page == null)
            throw new JsonException("Pet list could not be retrieved from Pet Finder API.");

        return petList;
    }

    /// <summary>
    ///     Retrieves a pet object containing attributes and adoption details.
    /// </summary>
    /// <param name="petId">Unique pet id to search for.</param>
    /// <returns>PetFinder Pet Response object if pet is found, else returns null.</returns>
    /// <exception cref="HttpRequestException">Throws if method can not make a successful call with PetFinder.</exception>
    public async Task<PetFinderPet?> GetSinglePetByIdAsync(int petId)
    {
        SetAuthenticationRequestHeadersAsync();
        using HttpResponseMessage response = await _httpClient.GetAsync($"{_httpClient.BaseAddress}/{petId}");

        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;
        response.EnsureSuccessStatusCode();

        var petResponse = await response.Content.ReadFromJsonAsync<PetFinderPetResponse>();
        if (petResponse is null)
            throw new JsonException(
                "Pet object could not be retrieved from Pet Finder API.");
        return petResponse.Pet;
    }

    private string GetPathWithQueryString(IPetSearchParameters petSearchParameters)
    {
        List<KeyValuePair<string, string?>> query =
        [
            new("type", petSearchParameters.Type),
            new("location", petSearchParameters.Location),
            new("page", petSearchParameters.Page.ToString()),
            new("distance", petSearchParameters.Distance.ToString()),
            new("sort", petSearchParameters.Sort)
        ];
        return QueryHelpers.AddQueryString(string.Empty, query);
    }

    private void SetAuthenticationRequestHeadersAsync()
    {
        PetFinderToken? token = _tokenCacheService.TryGetToken();
        if (token is null)
            throw new NullReferenceException("Token was not found in cache.");

        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, token.AccessToken);
    }
}