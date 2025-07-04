using System.Text.Json;
using DotnetPetSearch.API.Controllers;
using DotnetPetSearch.API.Extensions;
using DotnetPetSearch.API.Models;
using DotnetPetSearch.PetFinderHttpClient.Models;
using DotnetPetSearch.PetFinderHttpClient.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace DotnetPetSearch.API.Tests.Controllers;

public class PetsControllerTests
{
    private readonly IPetFinderClient _petFinderClient;
    private readonly PetsController _petsController;
    private readonly HeaderDictionary _headers;

    public PetsControllerTests()
    {
        _petFinderClient = Substitute.For<IPetFinderClient>();
        var mockHttpContext = Substitute.For<HttpContext>();
        _headers = new HeaderDictionary();
        _petsController = new PetsController(_petFinderClient)
        {
            ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext
            }
        };
        var httpContext = new DefaultHttpContext();
        mockHttpContext.Response.Headers.Returns(_headers);
    }

    [Fact]
    public async Task GetPetList_ReturnsOK_WhenClientIsSuccessful()
    {
        // Arrange
        _petFinderClient.GetPetsAsync(Arg.Any<PetSearchParameters>()).Returns(GetPetListResponse());
        // Action
        ActionResult<List<PetDto>> result = await _petsController.GetPetList(GetPetSearchParameters());
        // Assert
        Assert.IsType<OkObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetPetList_ReturnsPetListResult_WhenClientIsSuccessful()
    {
        // Arrange
        _petFinderClient.GetPetsAsync(Arg.Any<PetSearchParameters>()).Returns(GetPetListResponse());
        // Action
        ActionResult<List<PetDto>> result = await _petsController.GetPetList(GetPetSearchParameters());
        // Assert
        Assert.IsType<ActionResult<List<PetDto>>>(result);
    }

    [Fact]
    public async Task GetPetList_ReturnsPetListResult_ShouldReturnTheMirrorPetObjectFromPetFinderPetListResponse()
    {
        // Arrange
        var petListResponse = GetPetListResponse();
        _petFinderClient.GetPetsAsync(Arg.Any<PetSearchParameters>()).Returns(petListResponse);
        // Action
        ActionResult<List<PetDto>> result = await _petsController.GetPetList(GetPetSearchParameters());
        var okObjectResult = (OkObjectResult)result.Result!;
        var valueResult = (List<PetDto>)okObjectResult.Value!;
        var expectedPet = petListResponse.Animals.First();
        var actualPet = valueResult.First();
        // Assert
        Assert.Equivalent(expectedPet.Id, actualPet.Id);
        Assert.Equivalent(expectedPet.Name, actualPet.Name);
        Assert.Equivalent(expectedPet.Url, actualPet.Url);
        Assert.True(_headers.ContainsKey("X-Pagination"));
    }

    [Fact]
    public async Task GetPetList_SetsXPaginationHeaders()
    {
        var petListResponse = GetPetListResponse();
        // Arrange
        _petFinderClient.GetPetsAsync(Arg.Any<PetSearchParameters>()).Returns(petListResponse);
        var expectedSerializeHeader = JsonSerializer.Serialize(petListResponse.ToPaginationMetaData());
        // Action
        ActionResult<List<PetDto>> result = await _petsController.GetPetList(GetPetSearchParameters());
        // Assert
        Assert.True(_headers.ContainsKey("X-Pagination"));
        Assert.Equal(expectedSerializeHeader, _headers["X-Pagination"]);
    }

    [Fact]
    public async Task GetPetById_ReturnsOkObjectResult_WhenClientIsSuccessful()
    {
        // Arrange
        _petFinderClient.GetSinglePetByIdAsync(Arg.Any<int>()).Returns(GetPet());
        // Action
        ActionResult<PetDto> result = await _petsController.GetPetById(2);
        // Assert
        Assert.IsType<OkObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetPetById_ReturnsPetDto_WhenClientIsSuccessful()
    {
        // Arrange
        var petResponse = GetPet();
        _petFinderClient.GetSinglePetByIdAsync(Arg.Any<int>()).Returns(petResponse);
        // Action
        ActionResult<PetDto> result = await _petsController.GetPetById(2);
        var okObjectResult = (OkObjectResult)result.Result!;
        var valueResult = (PetDto)okObjectResult.Value!;
        // Assert
        Assert.IsType<PetDto>(valueResult);
        Assert.Equal(petResponse.Id, valueResult.Id);
        Assert.Equal(petResponse.Name, valueResult.Name);
        Assert.Equal(petResponse.Url, valueResult.Url);
    }

    [Fact]
    public async Task GetPetById_ReturnsNotFoundObjectResult_WhenClientIsUnsuccessful()
    {
        // Arrange
        _petFinderClient.GetSinglePetByIdAsync(Arg.Any<int>()).ReturnsNull();
        // Action
        ActionResult<PetDto> result = await _petsController.GetPetById(2);
        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
        
    }
    
private PetSearchParameters GetPetSearchParameters() =>
        new PetSearchParameters { Type = "Type", Location = "Location" };

    private PetFinderPetListResponse GetPetListResponse() =>
        new PetFinderPetListResponse
        {
            Animals = [GetPet()],
            Page = GetPetPageResponse()
        };

    private PetFinderPet GetPet() =>
        new PetFinderPet
        {
            Id = 1,
            Url = "Url",
            Type = "Type",
            Age = "Age",
            Gender = "Gender",
            Size = "Size",
            Name = "Name",
            Photos = [],
            Status = "Status",
        };

    private PetFinderPageResponse GetPetPageResponse() =>
        new PetFinderPageResponse
        {
            CountPerPage = 1,
            TotalCount = 1,
            CurrentPage = 1,
            TotalPages = 1
        };
}