using DotnetPetSearch.Data.Entities;

namespace DotnetPetSearch.PetFinderHttpClient.Services;

public interface IPetFinderTokenClient
{
    public Task<PetFinderToken?> TryGetTokenFromApiAsync(CancellationToken cancellationToken);
}