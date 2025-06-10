using DotnetPetSearch.Data.Entities;

namespace DotnetPetSearch.PetFinderHttpClient.Services;

/// <summary>
///     IPetFinderTokenService for retrieving a PetFinder token.
/// </summary>
public interface ICacheTokenService
{
    public Task<PetFinderToken?> GetToken();
    public void StoreTokenInCache(PetFinderToken token);
    public Task StoreTokenInDatabase(PetFinderToken token);
}