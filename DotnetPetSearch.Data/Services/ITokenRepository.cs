using DotnetPetSearch.Data.Entities;

namespace DotnetPetSearch.Data.Services;

public interface ITokenRepository
{
    public Task<PetFinderToken?> GetSingleTokenAsync();
    public Task SaveTokenAsync(PetFinderToken token);
}