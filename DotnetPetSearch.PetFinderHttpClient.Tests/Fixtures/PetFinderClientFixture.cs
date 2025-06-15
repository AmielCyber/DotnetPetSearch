using DotnetPetSearch.Data.Entities;
using DotnetPetSearch.PetFinderHttpClient.Models;

namespace DotnetPetSearch.PetFinderHttpClient.Tests.Fixtures;

public class PetFinderClientFixture
{
    public Uri BaseUri { get; } = new("http://petfinder.com/v2/animals");
    public PetFinderPetListResponse ExpectedPetListResponse { get; }
    public PetFinderPetResponse ExpectedPetResponse { get; }
    public PetFinderPet ExpectedPet { get; }
    public PetFinderPet? ExpectedEmptyPet { get; }

    public PetsSearchParameters ExpectedParameters { get; }
    public PetsSearchParameters ExpectedParametersWithDefaults { get; }
    public PetFinderToken ExpectedToken { get; }

    public PetFinderClientFixture()
    {
        ExpectedPetListResponse = GetPetList();
        ExpectedPetResponse = new PetFinderPetResponse() { Pet = GetPet() };
        ExpectedPet = GetPet();
        ExpectedEmptyPet = null;
        ExpectedParameters = new PetsSearchParameters
        {
            Type = "Type",
            Location = "Location",
            Page = 1,
            Distance = 50,
            SortBy = "recent"
        };
        ExpectedParametersWithDefaults = new PetsSearchParameters
        {
            Type = "type2",
            Location = "location2"
        };
        ExpectedToken = new PetFinderToken()
        {
            AccessToken = "AccessToken"
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