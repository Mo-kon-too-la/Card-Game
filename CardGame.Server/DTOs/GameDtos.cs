using CardGame.Infrastructure.Database.Entities;
using CardGame.Infrastructure.Shared;

namespace CardGame.Server.DTOs;

public class CardDto
{
    public Guid Id { get; set; }
    public Guid PlayerId { get; set; }
    public string Rank { get; set; } = null!;
    public string Suit { get; set; } = null!;
    public int Value { get; set; }
    public int SuitValue { get; set; }
    public int DeckId { get; set; }
}

public class ScoreDto
{
    public Guid Id { get; set; }
    public Guid PlayerId { get; set; }
    public int HandSum { get; set; }
    public long SuitProduct { get; set; }
    public bool IsTiedForHighestHand { get; set; }
    public bool IsWinner { get; set; }
}

public class PlayerDto
{
    public Guid Id { get; set; }
    public Guid GameId { get; set; }
    public int SeatNumber { get; set; }
    public string Name { get; set; } = null!;
    public List<CardDto> Cards { get; set; } = [];
    public ScoreDto? Score { get; set; }
}

public class GameDto
{
    public Guid Id { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public List<PlayerDto> Players { get; set; } = [];
}

public static class DtoMappingExtensions
{
    public static CardDto ToDto(this Card card) => new()
    {
        Id = card.Id,
        PlayerId = card.PlayerId,
        Rank = card.Rank,
        Suit = card.Suit,
        Value = card.Value,
        SuitValue = card.SuitValue,
        DeckId = card.DeckId
    };

    public static ScoreDto ToDto(this Score score) => new()
    {
        Id = score.Id,
        PlayerId = score.PlayerId,
        HandSum = score.HandSum,
        SuitProduct = score.SuitProduct,
        IsTiedForHighestHand = score.IsTiedForHighestHand,
        IsWinner = score.IsWinner
    };

    public static PlayerDto ToDto(this Player player) => new()
    {
        Id = player.Id,
        GameId = player.GameId,
        SeatNumber = player.SeatNumber,
        Name = player.Name,
        Cards = player.Cards.Select(c => c.ToDto()).ToList(),
        Score = player.Score?.ToDto()
    };

    public static GameDto ToDto(this Game game) => new()
    {
        Id = game.Id,
        CreatedAtUtc = game.CreatedAtUtc,
        Players = game.Players.OrderBy(p => p.SeatNumber).Select(p => p.ToDto()).ToList()
    };

    public static PagedResults<GameDto> ToDto(this PagedResults<Game> pagedGames) => new()
    {
        Items = pagedGames.Items.Select(g => g.ToDto()).ToList(),
        Page = pagedGames.Page,
        PageSize = pagedGames.PageSize,
        TotalCount = pagedGames.TotalCount
    };
}
