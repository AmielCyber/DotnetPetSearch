using System.Net.Http.Json;
using DotnetPetSearch.Data.Configurations;
using DotnetPetSearch.Data.Entities;
using DotnetPetSearch.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace DotnetPetSearch.Data.Services;

/// <summary>
///     TokenService implementation for retrieving a Token from the PetFinder API to use their API.
/// </summary>
public class TokenService : ITokenService
{
    private const int TokenId = 1;
    private const string TokenCacheKey = "PetFinderToken";
    private readonly PetSearchContext _context;
    private readonly PetFinderCredentials _credentials;
    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _memoryCache;

    public TokenService(IOptions<PetFinderCredentials> credentials,
        IMemoryCache memoryCache, PetSearchContext context, HttpClient httpClient)
    {
        _credentials = credentials.Value;
        _memoryCache = memoryCache;
        _context = context;
        _httpClient = httpClient;
    }

    /// <summary>
    ///     Retrieves a token from the memory cache or database in that order, if available and not expired.
    ///     Otherwise, it fetches a new token from the PetFinder API.
    /// </summary>
    /// <returns>
    ///     PetFinderToken containing the token and its expiration date.
    /// </returns>
    public async Task<PetFinderToken> GetTokenAsync()
    {
        PetFinderToken? token = GetTokenFromCache();
        if (token != null && TokenIsValid(token))
            return token;

        token = await GetTokenFromDatabaseAsync();
        if (token != null && TokenIsValid(token))
        {
            StoreTokenInCache(token);
            return token;
        }

        PetFinderToken newToken = await RequestNewTokenFromPetFinderAsync();
        StoreTokenInCache(newToken);
        await StoreTokenInDatabaseAsync(newToken);
        return newToken;
    }

    private static bool TokenIsValid(PetFinderToken token)
    {
        return DateTime.Now < token.ExpiresIn;
    }


    private PetFinderToken? GetTokenFromCache()
    {
        return _memoryCache.Get<PetFinderToken>(TokenCacheKey);
    }

    private async Task<PetFinderToken?> GetTokenFromDatabaseAsync()
    {
        return await _context.Tokens.AsNoTracking().FirstOrDefaultAsync(t => t.Id == TokenId);
    }

    private async Task<PetFinderToken> RequestNewTokenFromPetFinderAsync()
    {
        DateTime startTime = DateTime.Now;
        using HttpResponseMessage response =
            await _httpClient.PostAsJsonAsync(string.Empty, new PetFinderTokenRequest
            {
                ClientId = _credentials.ClientId,
                ClientSecret = _credentials.ClientSecret
            });
        response.EnsureSuccessStatusCode();
        var petFinderToken = await response.Content.ReadFromJsonAsync<TokenResponse>();
        if (petFinderToken is null)
            throw new HttpRequestException("Failed to obtain pet finder token!");

        DateTime expireTime = startTime.AddSeconds(petFinderToken.ExpiresIn);
        return new PetFinderToken
        {
            AccessToken = petFinderToken.AccessToken,
            ExpiresIn = expireTime
        };
    }

    private void StoreTokenInCache(PetFinderToken token)
    {
        MemoryCacheEntryOptions cacheEntryOptions =
            new MemoryCacheEntryOptions().SetAbsoluteExpiration(token.ExpiresIn);
        _memoryCache.Set(TokenCacheKey, token, cacheEntryOptions);
    }

    private async Task StoreTokenInDatabaseAsync(PetFinderToken updatedToken)
    {
        int rowsAffected = await _context.Tokens
            .Where(t => t.Id == updatedToken.Id)
            .ExecuteUpdateAsync(s => s.SetProperty(t => t.AccessToken, updatedToken.AccessToken));
        if (rowsAffected == 0)
        {
            await _context.Set<PetFinderToken>().AddAsync(updatedToken);
            await _context.SaveChangesAsync();
        }
    }
}