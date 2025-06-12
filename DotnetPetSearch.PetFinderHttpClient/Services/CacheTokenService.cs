using DotnetPetSearch.Data;
using DotnetPetSearch.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace DotnetPetSearch.PetFinderHttpClient.Services;

/// <summary>
///     TokenService implementation for retrieving a Token from the PetFinder API to use their API.
/// </summary>
public class CacheTokenService : ICacheTokenService
{
    private const int TokenId = 1;
    private const string TokenCacheKey = "token";
    private readonly PetSearchContext _context;
    private readonly IMemoryCache _memoryCache;

    public CacheTokenService(IMemoryCache memoryCache, PetSearchContext context)
    {
        _memoryCache = memoryCache;
        _context = context;
    }

    /// <summary>
    ///     Gets a token from either the memory cache or database in that order if there is one and is not expired.
    /// </summary>
    /// <returns>
    ///     PetFinderToken containing the token and its expiration date. Returns null if there is no cached token or is
    ///     expired.
    /// </returns>
    public async Task<PetFinderToken?> GetTokenAsync()
    {
        PetFinderToken? token = GetTokenFromCache();
        if (token is not null && !IsTokenExpired(token))
            return token;

        token = await GetTokenFromDatabaseAsync();
        if (token == null || IsTokenExpired(token)) return null;

        StoreTokenInCache(token);
        return token;
    }

    /// <summary>
    ///     Checks if a token is expired.
    /// </summary>
    /// <param name="token">The token to check expiration.</param>
    /// <returns>True if token is expired.</returns>
    public bool IsTokenExpired(PetFinderToken token)
    {
        return DateTime.Now >= token.ExpiresIn;
    }

    /// <summary>
    ///     Stores the passed token in the memory cache.
    /// </summary>
    /// <param name="token">The token to be stored in the memory cache.</param>
    public void StoreTokenInCache(PetFinderToken token)
    {
        MemoryCacheEntryOptions cacheEntryOptions =
            new MemoryCacheEntryOptions().SetAbsoluteExpiration(token.ExpiresIn);
        _memoryCache.Set(TokenCacheKey, token, cacheEntryOptions);
    }

    /// <summary>
    ///     Stores the passed token in the database.
    /// </summary>
    /// <param name="token">The token to be stored in the database.</param>
    /// <exception cref="DbUpdateException">Throws if failed to store in database.</exception>
    public async Task StoreTokenInDatabaseAsync(PetFinderToken token)
    {
        _context.Tokens.Update(token);
        await _context.SaveChangesAsync();
    }

    private PetFinderToken? GetTokenFromCache()
    {
        return _memoryCache.Get<PetFinderToken>(TokenCacheKey);
    }

    private async Task<PetFinderToken?> GetTokenFromDatabaseAsync()
    {
        return await _context.Tokens.AsNoTracking().FirstOrDefaultAsync(t => t.Id == TokenId);
    }
}