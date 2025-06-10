using System.Text.Json.Serialization;

namespace DotnetPetSearch.PetFinderHttpClient.Models;

/// <summary>
///     Bearer Token Request for PetFinderApi.
/// </summary>
/// <param name="ClientId">The Client ID key provided by PetFinderApi</param>
/// <param name="ClientSecret">The Client Secret key provided by PetFinderApi</param>
/// <param name="GrantType">Default to "client_credentials"</param>
public record PetFinderTokenRequest(
    [property: JsonPropertyName("client_id")]
    string ClientId,
    [property: JsonPropertyName("client_secret")]
    string ClientSecret,
    [property: JsonPropertyName("grant_type")]
    string GrantType = "client_credentials"
);