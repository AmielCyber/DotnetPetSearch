using DotnetPetSearch.PetFinderHttpClient.Models;

namespace DotnetPetSearch.PetFinderHttpClient.Services;

/// <summary>
///     Pet Finder Client interface to handle PetFinder API requests,
///     such as getting a list of pets and a single pet object from the PetFinder API.
/// </summary>
public interface IPetFinderClient
{
    public Task<PetFinderPetListResponse> GetPetsAsync(PetsSearchParameters petsSearchParameters);
    public Task<PetFinderPet?> GetSinglePetByIdAsync(int petId);
}