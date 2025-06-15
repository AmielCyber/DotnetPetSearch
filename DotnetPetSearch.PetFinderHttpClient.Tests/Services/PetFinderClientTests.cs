using DotnetPetSearch.PetFinderHttpClient.Tests.Fixtures;

namespace DotnetPetSearch.PetFinderHttpClient.Tests.Services;

public class PetFinderClientTests: IClassFixture<PetFinderClientFixture>
{
    private readonly PetFinderClientFixture _fixture;
    
    public PetFinderClientTests(PetFinderClientFixture fixture)
    {
        _fixture = fixture;
    }

    public async Task GetPetsAsync_ShouldSetAuthenticationRequestHeaders_ToHttpClient()
    {
    }

    public async Task GetPetsAsync_ShouldPassQueryParametersToRequest()
    {
        
    }
    public async Task GetPetsAsync_ShouldPassQueryParametersToRequest_WithDefaultParameters()
    {
        
    }
    public async Task GetPetsAsync_ShouldThrowHttpRequestException_WhenResponseIsUnsuccessful()
    {
    }
    public async Task GetPetsAsync_ShouldThrowHttpRequestException_WhenResponseContentFailsToExtractBody()
    {
    }
    
    public async Task GetPetsAsync_ShouldReturn_APetListResponse_WhenResponseIsSuccessful()
    {
    }
    public async Task GetSinglePetByIdAsync_ShouldSetAuthenticationRequestHeaders_ToHttpClient()
    {
    }
    public async Task GetSinglePetByIdAsync_ShouldPassIdToRequest()
    {
    }
    public async Task GetSinglePetByIdAsync_ReturnNull_WhenResponseStatusCodeIsNotFound()
    {
    }
    public async Task GetSinglePetByIdAsync_ShouldThrowHttpRequestException_WhenResponseIsUnsuccessful()
    {
        
    }
    public async Task GetSinglePetByIdAsync_ShouldThrowHttpRequestException_WhenResponseContentFailsToExtractBody()
    {
        
    }
    public async Task GetSinglePetByIdAsync_ShouldReturn_APetListResponse_WhenResponseIsSuccessful()
    {
    }

    
}