using System.Data.Common;
using DotnetPetSearch.Data.Configurations;
using DotnetPetSearch.Data.Entities;
using DotnetPetSearch.Data.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DotnetPetSearch.Data.Tests.Fixtures;

public class TokenServiceFixture : IDisposable
{
    private readonly DbConnection _dbConnection;

    public IOptions<PetFinderCredentials> ExpectedCredentialOptions { get; }
    public DbContextOptions<PetSearchContext> DbContextOptions { get; init; }
    public readonly Uri BaseUri = new("http://petfinder.com/v2/token");
    public readonly string TokenFromCache = "cache";
    public readonly string TokenFromDatabase = "database";
    public readonly string TokenFromServer = "server";
    public PetFinderTokenRequest ExpectedTokenRequestBody;

    public TokenServiceFixture()
    {
        ExpectedCredentialOptions = Options.Create(new PetFinderCredentials()
        {
            ClientId = "client",
            ClientSecret = "secret"
        });
        ExpectedTokenRequestBody = new PetFinderTokenRequest
        {
            ClientId = ExpectedCredentialOptions.Value.ClientId,
            ClientSecret = ExpectedCredentialOptions.Value.ClientSecret
        };
        _dbConnection = new SqliteConnection("Filename=:memory:");
        _dbConnection.Open();
        DbContextOptions = new DbContextOptionsBuilder<PetSearchContext>()
            .UseSqlite(_dbConnection)
            .Options;
    }

    public void Dispose()
    {
        _dbConnection.Dispose();
    }
}