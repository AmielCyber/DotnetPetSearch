using System.Net.Http.Json;
using System.Text.Json;
using DotnetPetSearch.Data.Configurations;
using DotnetPetSearch.Data.Entities;
using DotnetPetSearch.Data.Models;
using Microsoft.Extensions.Options;

namespace DotnetPetSearch.PetFinderHttpClient.Services;

public class PetFinderTokenClient : IPetFinderTokenClient
{
    private readonly HttpClient _httpClient;
    private readonly PetFinderCredentials _credentials;

    public PetFinderTokenClient(HttpClient httpClient, IOptions<PetFinderCredentials> credentials)
    {
        _httpClient = httpClient;
        _credentials = credentials.Value;
    }

    public async Task<PetFinderToken> TryGetTokenFromApiAsync(CancellationToken cancellationToken)
    {
        DateTime startTime = DateTime.Now;
        using HttpResponseMessage response =
            await _httpClient.PostAsJsonAsync(string.Empty, new PetFinderTokenRequest
            {
                ClientId = _credentials.ClientId,
                ClientSecret = _credentials.ClientSecret
            }, cancellationToken);
        response.EnsureSuccessStatusCode();

        var petFinderToken = await response.Content.ReadFromJsonAsync<TokenResponse>(cancellationToken);
        if (petFinderToken is null)
            throw new JsonException("Failed to read Token Response.");

        DateTime expireTime = startTime.AddSeconds(petFinderToken.ExpiresIn);
        return new PetFinderToken
        {
            AccessToken = petFinderToken.AccessToken,
            ExpiresIn = expireTime
        };
    }
}