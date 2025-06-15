using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using DotnetPetSearch.Data.Entities;
using DotnetPetSearch.Data.Services;
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
    private readonly ITokenService _tokenService;

    public PetFinderClient(HttpClient httpClient, ITokenService tokenService)
    {
        _httpClient = httpClient;
        _tokenService = tokenService;
    }

    /// <summary>
    ///     Gets a list of available pets along with its pagination data based on the passed query parameters.
    /// </summary>
    /// <param name="petsSearchParameters">Query parameters for the pet search.</param>
    /// <returns>PetFinder Pet List response which contains a list of pets and pagination metadata.</returns>
    /// <exception cref="HttpRequestException">Throws if method can not make a successful call with PetFinder.</exception>
    public async Task<PetFinderPetListResponse> GetPetsAsync(PetsSearchParameters petsSearchParameters)
    {
        await SetAuthenticationRequestHeadersAsync();
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
    public async Task<PetFinderPet?> GetSinglePetByIdAsync(int petId)
    {
        await SetAuthenticationRequestHeadersAsync();
        using HttpResponseMessage response = await _httpClient.GetAsync($"/{petId}");

        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;
        response.EnsureSuccessStatusCode();

        PetFinderPetResponse petResponse = await response.Content.ReadFromJsonAsync<PetFinderPetResponse>()
                                           ?? throw new HttpRequestException(
                                               "Pet object could not be retrieved from Pet Finder API.");
        return petResponse.Pet;
    }

    private string GetPathWithQueryString(PetsSearchParameters petsSearchParameters)
    {
        List<KeyValuePair<string, string?>> query =
        [
            new("type", petsSearchParameters.Type),
            new("location", petsSearchParameters.Location),
            new("page", petsSearchParameters.Page.ToString()),
            new("distance", petsSearchParameters.Distance.ToString()),
            new("sort", petsSearchParameters.SortBy)
        ];
        return QueryHelpers.AddQueryString(String.Empty, query);
    }

    private async Task SetAuthenticationRequestHeadersAsync()
    {
        PetFinderToken token = await _tokenService.GetTokenAsync();
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, token.AccessToken);
    }

}