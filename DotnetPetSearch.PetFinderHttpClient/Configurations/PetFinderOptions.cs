namespace DotnetPetSearch.PetFinderHttpClient.Configurations;

/// <summary>
///     Pet Finder API resource locator declared in appsettings.
/// </summary>
public class PetFinderOptions
{
    public required string BaseUri { get; init; }
    public required string PetSearchUrl { get; init; }
    public required string TokenUrl { get; init; }
}