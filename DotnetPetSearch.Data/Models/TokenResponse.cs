using System.Text.Json.Serialization;

namespace DotnetPetSearch.Data.Models;

/// <summary>
///     Token Response DTO for client. Tells client when that their token will expire.
/// </summary>
public class TokenResponse
{
    /// <summary>Token Type: Bearer</summary>
    [JsonPropertyName("token_type")]
    public required string TokenType { get; init; }

    /// <summary>Expires in seconds when obtained from the back end.</summary>
    [JsonPropertyName("expires_in")]
    public required int ExpiresIn { get; init; }

    /// <summary>The token string</summary>
    [JsonPropertyName("access_token")]
    public required string AccessToken { get; init; }
}