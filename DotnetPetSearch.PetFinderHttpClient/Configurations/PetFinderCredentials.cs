namespace DotnetPetSearch.PetFinderHttpClient.Configurations;

/// <summary>
///     Pet Finder Credentials in order to use the Pet Finder API. Declared in Azure vault (production)
///     or in user-secrets (development).
/// </summary>
public class PetFinderCredentials
{
    /// <summary>
    ///     Client id provided by PetFinder API
    /// </summary>
    public required string ClientId { get; init; }

    /// <summary>
    ///     Client secret provided by PetFinder API
    /// </summary>
    public required string ClientSecret { get; init; }
}