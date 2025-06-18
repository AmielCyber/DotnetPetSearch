using DotnetPetSearch.API.Models;
using DotnetPetSearch.PetFinderHttpClient.Models;

namespace DotnetPetSearch.API.Extensions;

public static class PetFinderPetResponseExtensions
{
    public static PetDto ToPetDto(this PetFinderPet petFinderPet)
    {
        return new PetDto
        {
            Id = petFinderPet.Id,
            Url = petFinderPet.Url,
            Type = petFinderPet.Type,
            Age = petFinderPet.Age,
            Gender = petFinderPet.Gender,
            Size = petFinderPet.Size,
            Name = petFinderPet.Name,
            Description = petFinderPet.Description,
            Photos = petFinderPet.Photos,
            PrimaryPhotoCropped = petFinderPet.PrimaryPhotoCropped,
            Status = petFinderPet.Status,
            Distance = petFinderPet.Distance
        };
    }

    public static List<PetDto> ToPetListDto(this PetFinderPetListResponse petListResponse)
    {
        return petListResponse.Animals.Select(p => p.ToPetDto()).ToList();
    }

    public static PaginationMetaData ToPaginationMetaData(this PetFinderPetListResponse petListResponse)
    {
        return new PaginationMetaData(
            petListResponse.Page.CurrentPage,
            petListResponse.Page.TotalPages,
            petListResponse.Page.CountPerPage,
            petListResponse.Page.TotalCount
        );
    }
}