using DotnetPetSearch.Data.Entities;
using DotnetPetSearch.Data.Services;
using DotnetPetSearch.PetFinderHttpClient.Services;

namespace DotnetPetSearch.API.Services;

public class TokenRefreshService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private const int TokenRefreshBuffer = 2;
    private const int MaxTokenRequestRetries = 2;
    private readonly TimeSpan _retryTimeOut;
    private TimeSpan _refreshTokenDelay;
    private readonly ITokenCacheService _tokenCacheService;
    private readonly ILogger<TokenRefreshService> _logger;

    public TokenRefreshService(IServiceProvider serviceProvider, ITokenCacheService tokenCacheService,
        ILogger<TokenRefreshService> logger)
    {
        _serviceProvider = serviceProvider;
        _tokenCacheService = tokenCacheService;
        _logger = logger;
        _retryTimeOut = TimeSpan.FromMinutes(10);
        _refreshTokenDelay = _retryTimeOut;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        var retryAttempt = 0;
        PetFinderToken? petFinderToken = null;
        while (!cancellationToken.IsCancellationRequested && petFinderToken == null &&
               retryAttempt < MaxTokenRequestRetries)
        {
            DateTime startRequest = DateTime.Now.AddMinutes(TokenRefreshBuffer);
            petFinderToken = await TryRefreshTokenInCache(cancellationToken);
            if (petFinderToken != null)
                _refreshTokenDelay = petFinderToken.ExpiresIn - startRequest;
            else
                retryAttempt++;
        }

        await base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            await Task.Delay(_refreshTokenDelay, cancellationToken);
            DateTime startRequest = DateTime.Now.AddMinutes(TokenRefreshBuffer);
            PetFinderToken? petFinderToken = await TryRefreshTokenInCache(cancellationToken);
            if (petFinderToken != null)
                _refreshTokenDelay = petFinderToken.ExpiresIn - startRequest;
            else
                _refreshTokenDelay = _retryTimeOut;
        }
    }

    private async Task<PetFinderToken?> TryRefreshTokenInCache(CancellationToken cancellationToken)
    {
        using IServiceScope scope = _serviceProvider.CreateScope();
        IServiceProvider scopedProvider = scope.ServiceProvider;
        var tokenClient = scopedProvider.GetRequiredService<IPetFinderTokenClient>();
        var tokenRepository = scopedProvider.GetRequiredService<ITokenRepository>();

        PetFinderToken? token = await tokenRepository.GetSingleTokenAsync();
        if (token != null && !IsTokenExpired(token))
        {
            _tokenCacheService.StoreToken(token);
            return token;
        }
        try
        {
            token = await tokenClient.TryGetTokenFromApiAsync(cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError("Failed to retrieve token from PetFinder API.");
            _logger.LogError(e, e.Message);
        }

        if (token == null) return token;

        _tokenCacheService.StoreToken(token);
        await tokenRepository.SaveTokenAsync(token);
        return token;
    }


    private static bool IsTokenExpired(PetFinderToken token)
    {
        return DateTime.Now >= token.ExpiresIn;
    }
}