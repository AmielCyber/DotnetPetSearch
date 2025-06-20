using DotnetPetSearch.Data.Entities;
using Microsoft.Extensions.Caching.Memory;

namespace DotnetPetSearch.PetFinderHttpClient.Services;

public class TokenCacheService : ITokenCacheService
{
    private const string CacheKey = "PetFinderToken";
    private readonly IMemoryCache _cache;

    public TokenCacheService(IMemoryCache memoryCache)
    {
        _cache = memoryCache;
    }

    public PetFinderToken? TryGetToken()
    {
        return _cache.Get<PetFinderToken>(CacheKey);
    }

    public void StoreToken(PetFinderToken token)
    {
        _cache.Set(CacheKey, token);
    }
}