using DotnetPetSearch.API.Controllers;
using DotnetPetSearch.PetFinderHttpClient.Services;
using NSubstitute;

namespace DotnetPetSearch.API.Tests.Controllers;

public class PetsControllerTests
{
    private readonly IPetFinderClient _petFinderClient;
    private readonly PetsController _petsController;
    
    public PetsControllerTests()
    {
        _petFinderClient = Substitute.For<IPetFinderClient>();
        _petsController = new PetsController(_petFinderClient);
    }
}