using DotnetPetSearch.Data.Entities;

namespace DotnetPetSearch.Data.Services;

/// <summary>
///     IPetFinderTokenService for retrieving a PetFinder token.
/// </summary>
public interface ITokenService
{
    public Task<PetFinderToken> GetTokenAsync();
}