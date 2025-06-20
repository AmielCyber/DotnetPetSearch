using DotnetPetSearch.Data.Entities;
using DotnetPetSearch.Data.Services;
using DotnetPetSearch.Data.Tests.Fixtures;
using Microsoft.EntityFrameworkCore;

namespace DotnetPetSearch.Data.Tests.Services;

public class TokenRepositoryTests : IClassFixture<TokenRepositoryFixture>
{
    private readonly PetSearchContext _context;
    private readonly TokenRepositoryFixture _fixture;

    public TokenRepositoryTests(TokenRepositoryFixture fixture)
    {
        _fixture = fixture;
        _context = new PetSearchContext(_fixture.DbContextOptions);
    }

    [Fact]
    public async Task GetSingleTokenAsync_ShouldReturnToken_IfTokenExists()
    {
        // Arrange
        await CreateDatabase(_fixture.ExpectedTokenInDatabase);
        ITokenRepository repository = new TokenRepository(_context);
        // Action
        PetFinderToken? result = await repository.GetSingleTokenAsync();
        // Assert
        Assert.NotNull(result);
        Assert.Equivalent(_fixture.ExpectedTokenInDatabase, result);
    }

    [Fact]
    public async Task GetSingleTokenAsync_ShouldReturnNull_IfTokenDoesNotExists()
    {
        // Arrange
        await CreateDatabase(null);
        ITokenRepository repository = new TokenRepository(_context);
        // Action
        PetFinderToken? result = await repository.GetSingleTokenAsync();
        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task SaveTokenAsync_ShouldSaveUpdatedToken_IfTokenExists()
    {
        // Arrange
        await CreateDatabase(_fixture.ExpectedTokenInDatabase);
        ITokenRepository repository = new TokenRepository(_context);
        // Action
        await repository.SaveTokenAsync(_fixture.ExpectedUpdatedToken);
        PetFinderToken? result = await _context.Tokens.SingleAsync();
        // Assert
        Assert.NotEqual(_fixture.ExpectedTokenInDatabase, _fixture.ExpectedUpdatedToken);
        Assert.Equivalent(_fixture.ExpectedUpdatedToken, result);
    }

    [Fact]
    public async Task SaveTokenAsync_ShouldSaveUpdatedToken_IfNoTokenExists()
    {
        // Arrange
        await CreateDatabase(null);
        ITokenRepository repository = new TokenRepository(_context);
        // Action
        await repository.SaveTokenAsync(_fixture.ExpectedUpdatedToken);
        PetFinderToken? result = await _context.Tokens.SingleOrDefaultAsync();
        // Assert
        Assert.Equivalent(_fixture.ExpectedUpdatedToken, result);
    }

    private async Task CreateDatabase(PetFinderToken? tokenInDatabase)
    {
        await _context.Database.EnsureDeletedAsync();
        await _context.Database.EnsureCreatedAsync();
        if (tokenInDatabase is null) return;
        await _context.Tokens.AddAsync(tokenInDatabase);
        await _context.SaveChangesAsync();
    }
}