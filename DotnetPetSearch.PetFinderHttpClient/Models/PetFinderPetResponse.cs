using System.Text.Json.Serialization;

namespace DotnetPetSearch.PetFinderHttpClient.Models;

public class PetFinderPetResponse
{
    [JsonPropertyName("animal")] public required PetFinderPet Pet { get; set; }
}