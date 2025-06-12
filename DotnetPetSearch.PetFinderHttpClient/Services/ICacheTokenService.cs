using DotnetPetSearch.Data.Entities;

namespace DotnetPetSearch.PetFinderHttpClient.Services;

/// <summary>
///     IPetFinderTokenService for retrieving a PetFinder token.
/// </summary>
public interface ICacheTokenService
{
    public Task<PetFinderToken?> GetTokenAsync();
    public bool IsTokenExpired(PetFinderToken token);
    public void StoreTokenInCache(PetFinderToken token);
    public Task StoreTokenInDatabaseAsync(PetFinderToken token);
}