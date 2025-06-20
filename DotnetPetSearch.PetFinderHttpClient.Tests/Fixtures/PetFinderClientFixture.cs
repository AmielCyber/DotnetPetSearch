using DotnetPetSearch.Data.Entities;
using DotnetPetSearch.PetFinderHttpClient.Models;

namespace DotnetPetSearch.PetFinderHttpClient.Tests.Fixtures;

public class PetSearchParamsImpl : IPetSearchParameters
{
    public string Type { get; init; } = "Type";
    public string Location { get; init; } = "Location";
    public int Page { get; init; } = 1;
    public int Distance { get; init; } = 25;
    public string Sort { get; init; } = "Sort";
}

public class PetFinderClientFixture
{
    public Uri BaseUri { get; } = new("http://petfinder.com/v2/animals");
    public PetFinderPetListResponse ExpectedPetListResponse { get; }
    public PetFinderPetResponse ExpectedPetResponse { get; }
    public PetFinderPet ExpectedPet { get; }
    public PetFinderPet? ExpectedEmptyPet { get; }

    public IPetSearchParameters ExpectedParameters { get; }
    public IPetSearchParameters ExpectedParametersWithDefaults { get; }
    public PetFinderToken ExpectedToken { get; }


    public PetFinderClientFixture()
    {
        ExpectedPetListResponse = GetPetList();
        ExpectedPetResponse = new PetFinderPetResponse() { Pet = GetPet() };
        ExpectedPet = GetPet();
        ExpectedEmptyPet = null;
        ExpectedParameters = new PetSearchParamsImpl();
        ExpectedParametersWithDefaults = new PetSearchParamsImpl()
        {
            Type = "type2",
            Location = "location2"
        };
        ExpectedToken = new PetFinderToken()
        {
            AccessToken = "AccessToken",
            ExpiresIn = DateTime.Now.AddHours(1)
        };
    }

    private static PetFinderPetListResponse GetPetList()
    {
        var petList = new List<PetFinderPet>();
        for (var i = 0; i < 5; i++) petList.Add(GetPet());
        return new PetFinderPetListResponse()
        {
            Animals = petList,
            Page = new PetFinderPageResponse
            {
                CountPerPage = 1,
                TotalCount = 1,
                CurrentPage = 1,
                TotalPages = 1
            }
        };
    }

    private static PetFinderPet GetPet()
    {
        return
            new PetFinderPet()
            {
                Url = "url",
                Type = "type",
                Age = "age",
                Gender = "gender",
                Size = "size",
                Name = "name",
                Photos = new List<PhotoSizeUrl>()
                {
                    new()
                    {
                        Small = "small",
                        Medium = "med",
                        Large = "lg",
                        Full = "full"
                    }
                },
                Status = "status"
            };
    }
}