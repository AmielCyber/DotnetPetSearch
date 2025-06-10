namespace DotnetPetSearch.PetFinderHttpClient.Models;

/// <summary>
///     PetList response from the PetFinder API.
///     Does not include sensitive data such as phone numbers, emails, since we will not send sensitive data to the client.
/// </summary>
/// <param name="Animals">The pet list.</param>
/// <param name="Page">The pagination metadata from the PetFinder API JSON response.</param>
public record PetFinderPetListResponse(List<PetFinderPetResponse> Animals, PetFinderPageResponse Page);