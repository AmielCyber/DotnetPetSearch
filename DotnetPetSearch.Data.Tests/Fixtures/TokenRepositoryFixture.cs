using System.Data.Common;
using DotnetPetSearch.Data.Entities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace DotnetPetSearch.Data.Tests.Fixtures;

public class TokenRepositoryFixture : IDisposable
{
    private readonly DbConnection _dbConnection;

    public DbContextOptions<PetSearchContext> DbContextOptions { get; }
    public PetFinderToken ExpectedTokenInDatabase { get; }
    public PetFinderToken ExpectedUpdatedToken { get; }

    public TokenRepositoryFixture()
    {
        _dbConnection = new SqliteConnection("Filename=:memory:");
        _dbConnection.Open();
        DbContextOptions = new DbContextOptionsBuilder<PetSearchContext>()
            .UseSqlite(_dbConnection)
            .Options;
        ExpectedTokenInDatabase = new PetFinderToken
        {
            AccessToken = "token_in_database",
            ExpiresIn = DateTime.Now.AddSeconds(3600)
        };
        ExpectedUpdatedToken = new PetFinderToken()
        {
            AccessToken = "updated access token",
            ExpiresIn = DateTime.Now
        };
    }

    public void Dispose()
    {
        _dbConnection.Dispose();
    }
}