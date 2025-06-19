namespace DotnetPetSearch.PetFinderHttpClient.Models;

public interface IPetSearchParameters
{
    public string Type { get; init; }
    public string Location { get; init; }
    public int Page { get; init; }
    public int Distance { get; init; }
    public string Sort { get; init; }
}