using DotnetPetSearch.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace DotnetPetSearch.Data.Services;

public class TokenRepository : ITokenRepository
{
    private readonly PetSearchContext _context;

    public TokenRepository(PetSearchContext context) => _context = context;

    public async Task<PetFinderToken?> GetSingleTokenAsync() =>
        await _context.Tokens.FirstOrDefaultAsync();

    public async Task SaveTokenAsync(PetFinderToken token)
    {
        PetFinderToken? tokenEntity = await _context.Tokens.FirstOrDefaultAsync();
        if (tokenEntity == null)
        {
            _context.Tokens.Add(token);
        }
        else
        {
            tokenEntity.AccessToken = token.AccessToken;
            tokenEntity.ExpiresIn = token.ExpiresIn;
        }

        await _context.SaveChangesAsync();
    }
}