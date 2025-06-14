using System.Text.Json.Serialization;

namespace DotnetPetSearch.Data.Models;

/// <summary>
///     Bearer Token Request Body for PetFinderApi.
/// </summary>
public class PetFinderTokenRequest
{
    /// <summary>The Client ID key provided by PetFinderApi</summary>
    [JsonPropertyName("client_id")]
    public required string ClientId { get; init; }

    /// <summary>The Client Secret key provided by PetFinderApi</summary>
    [JsonPropertyName("client_secret")]
    public required string ClientSecret { get; init; }

    /// <summary>Default to "client_credentials"</summary>
    [JsonPropertyName("grant_type")]
    public string GrantType { get; init; } = "client_credentials";

}