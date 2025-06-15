using DotnetPetSearch.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace DotnetPetSearch.Data;

/// <summary>
///     Our Context to store a temporary PetFinder token in our DB.
/// </summary>
public class PetSearchContext : DbContext
{
    public PetSearchContext(DbContextOptions<PetSearchContext> options) : base(options)
    {
    }

    public DbSet<PetFinderToken> Tokens { get; set; }
}