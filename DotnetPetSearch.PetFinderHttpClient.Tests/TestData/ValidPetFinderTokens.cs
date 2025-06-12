using DotnetPetSearch.Data.Entities;

namespace DotnetPetSearch.PetFinderHttpClient.Tests.TestData;

public class ValidPetFinderTokens: TheoryData<PetFinderToken>
{
    public ValidPetFinderTokens()
    {
        Add(new PetFinderToken()
        {
            AccessToken = string.Empty,
            ExpiresIn = DateTime.Now.AddYears(1)
        });
        Add(new PetFinderToken()
        {
            AccessToken = string.Empty,
            ExpiresIn = DateTime.Now.AddMonths(1)
        });
        Add(new PetFinderToken()
        {
            AccessToken = string.Empty,
            ExpiresIn = DateTime.Now.AddDays(1)
        });
        Add(new PetFinderToken()
        {
            AccessToken = string.Empty,
            ExpiresIn = DateTime.Now.AddHours(1)
        });
        Add(new PetFinderToken()
        {
            AccessToken = string.Empty,
            ExpiresIn = DateTime.Now.AddMinutes(1)
        });
    }
    
}