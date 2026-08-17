using CardGame.Infrastructure.Database.Entities;
using CardGame.Infrastructure.Shared;

namespace CardGame.Infrastructure.Services;

public interface IGameService
{
    Task<Game> CreateGameAsync(List<string> playerNames);
    Task<Game?> ReDealGameAsync(Guid gameId);
    Task<Game?> GetGameByIdAsync(Guid gameId);
    Task<PagedResults<Game>> GetPaginatedGamesAsync(
        int page,
        int pageSize,
        string? sortBy = "date",
        string? sortDirection = "desc",
        string? filterPlayerName = null);
}

