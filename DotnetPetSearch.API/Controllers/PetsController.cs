using System.Net.Mime;
using System.Text.Json;
using DotnetPetSearch.API.Extensions;
using DotnetPetSearch.API.Models;
using DotnetPetSearch.PetFinderHttpClient.Models;
using DotnetPetSearch.PetFinderHttpClient.Services;
using Microsoft.AspNetCore.Mvc;

namespace DotnetPetSearch.API.Controllers;

[ApiController]
[Route("/api/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
public class PetsController : ControllerBase
{
    private readonly IPetFinderClient _petFinderClient;

    public PetsController(IPetFinderClient petFinderClient)
    {
        _petFinderClient = petFinderClient;
    }

    /// <summary>
    /// Retrieve a list of available pets for adoption within a radius of the passed query.
    /// </summary>
    /// <param name="parameters">The query parameters to fine tune your search.</param>
    /// <returns>A pet list from the given passed location and other parameters.</returns>
    [HttpGet]
    [ResponseCache(Location = ResponseCacheLocation.Client, Duration = 600)]
    public async Task<ActionResult<List<PetDto>>> GetPetList([FromQuery] PetSearchParameters parameters)
    {
        PetFinderPetListResponse petListResponse = await _petFinderClient.GetPetsAsync(parameters);
        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(petListResponse.ToPaginationMetaData()));
        return petListResponse.ToPetListDto();
    }

    /// <summary>
    /// Retrieve a pet by its identification.
    /// </summary>
    /// <param name="petId">The pet's ID number</param>
    /// <returns>A pet object containing its attributes and adoption information.</returns>
    [HttpGet("{petId}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PetDto>> GetPetById(int petId)
    {
        PetFinderPet? petResponse = await _petFinderClient.GetSinglePetByIdAsync(petId);
        return petResponse == null ? NotFound() : Ok(petResponse.ToPetDto());
    }
}