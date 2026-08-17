using CardGame.Infrastructure.Database;
using CardGame.Infrastructure.Database.Entities;
using CardGame.Infrastructure.Shared;
using Microsoft.EntityFrameworkCore;

namespace CardGame.Infrastructure.Services;

public class GameService : IGameService
{
    private readonly CardGameDbContext _dbContext;
    private readonly IDeckService _deckService;
    private readonly IScoringEngine _scoringEngine;

    public GameService(CardGameDbContext dbContext, IDeckService deckService, IScoringEngine scoringEngine)
    {
        _dbContext = dbContext;
        _deckService = deckService;
        _scoringEngine = scoringEngine;
    }
    public async Task<Game> CreateGameAsync(List<string> playerNames)
    {
        var game = new Game();

        var players = _deckService.DealCards(playerNames);
        foreach (var player in players)
        {
            player.GameId = game.Id;
        }

        _scoringEngine.CalculateScoresAndWinners(players);

        game.Players = players;

        _dbContext.Games.Add(game);
        await _dbContext.SaveChangesAsync();

        return game;
    }

    public async Task<Game?> GetGameByIdAsync(Guid gameId)
    {
        return await _dbContext.Games
            .AsNoTracking()
            .AsSplitQuery()
            .Include(g => g.Players.OrderBy(p => p.SeatNumber))
                .ThenInclude(p => p.Cards)
            .Include(g => g.Players)
                .ThenInclude(p => p.Score)
            .FirstOrDefaultAsync(g => g.Id == gameId);
    }

    public async Task<PagedResults<Game>> GetPaginatedGamesAsync(
        int page,
        int pageSize,
        string? sortBy = "date",
        string? sortDirection = "desc",
        string? filterPlayerName = null)
    {
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 50) pageSize = 10;

        var query = _dbContext.Games.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(filterPlayerName))
        {
            var nameLower = filterPlayerName.ToLower();
            query = query.Where(g => g.Players.Any(p => p.Name.ToLower().Contains(nameLower) && p.Score!.IsWinner));
        }

        int totalCount = await query.CountAsync();

        var sortByLower = sortBy?.ToLower() ?? "date";
        var sortAsc = sortDirection?.ToLower() == "asc";

        IOrderedQueryable<Game> orderedQuery = (sortByLower, sortAsc) switch
        {
            ("score", true) => query.OrderBy(g => g.Players.Max(p => p.Score != null ? p.Score.HandSum : 0)),
            ("score", false) => query.OrderByDescending(g => g.Players.Max(p => p.Score != null ? p.Score.HandSum : 0)),
            ("playername", true) => query.OrderBy(g => g.Players.Where(p => p.Score != null && p.Score.IsWinner).Select(p => p.Name).FirstOrDefault() ?? string.Empty),
            ("playername", false) => query.OrderByDescending(g => g.Players.Where(p => p.Score != null && p.Score.IsWinner).Select(p => p.Name).FirstOrDefault() ?? string.Empty),
            (_, true) => query.OrderBy(g => g.CreatedAtUtc),
            _ => query.OrderByDescending(g => g.CreatedAtUtc)
        };

        var items = await orderedQuery
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .AsSplitQuery()
            .Include(g => g.Players.OrderBy(p => p.SeatNumber))
                .ThenInclude(p => p.Cards)
            .Include(g => g.Players)
                .ThenInclude(p => p.Score)
            .ToListAsync();

        return new PagedResults<Game>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<Game?> ReDealGameAsync(Guid gameId)
    {
        var game = await _dbContext.Games
            .Include(g => g.Players)
            .FirstOrDefaultAsync(g => g.Id == gameId);

        if (game == null)
        {
            return null;
        }

        var players = game.Players.OrderBy(p => p.SeatNumber).ToList();
        var playerIds = players.Select(p => p.Id).ToList();
        var names = players.Select(p => p.Name).ToList();

        _dbContext.Cards.RemoveRange(_dbContext.Cards.Where(c => playerIds.Contains(c.PlayerId)));
        _dbContext.Scores.RemoveRange(_dbContext.Scores.Where(s => playerIds.Contains(s.PlayerId)));

        foreach (var player in players)
        {
            player.Cards.Clear();
            player.Score = null;
        }

        var dealt = _deckService.DealCards(names).OrderBy(p => p.SeatNumber).ToList();
        var cards = new List<Card>(30);

        for (int i = 0; i < players.Count; i++)
        {
            var p = players[i];
            var d = dealt[i];

            foreach (var card in d.Cards)
            {
                card.PlayerId = p.Id;
                p.Cards.Add(card);
                cards.Add(card);
            }
        }

        _scoringEngine.CalculateScoresAndWinners(game.Players);

        var scores = game.Players.Where(p => p.Score != null).Select(p => p.Score!).ToList();

        foreach (var card in cards)
        {
            _dbContext.Entry(card).State = EntityState.Added;
        }
        foreach (var score in scores)
        {
            _dbContext.Entry(score).State = EntityState.Added;
        }

        await _dbContext.SaveChangesAsync();

        return game;
    }
}
