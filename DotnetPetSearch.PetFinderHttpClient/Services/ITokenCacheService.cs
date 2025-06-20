using DotnetPetSearch.Data.Entities;

namespace DotnetPetSearch.PetFinderHttpClient.Services;

public interface ITokenCacheService
{
    public PetFinderToken? TryGetToken();
    public void StoreToken(PetFinderToken token);
}