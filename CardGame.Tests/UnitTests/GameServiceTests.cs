using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CardGame.Infrastructure.Database;
using CardGame.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CardGame.Tests.UnitTests;

public class GameServiceTests
{
    private CardGameDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<CardGameDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new CardGameDbContext(options);
    }

    [Fact]
    public async Task CreateGameAsync_DealsCardsAndSavesGameToDb()
    {
        // Arrange
        using var dbContext = CreateDbContext();
        var deckService = new DeckService();
        var scoringEngine = new ScoringEngineService();
        var gameService = new GameService(dbContext, deckService, scoringEngine);

        var playerNames = new List<string> { "Alice", "Bob", "Carol", "Dave", "Eve", "Frank" };

        // Act
        var game = await gameService.CreateGameAsync(playerNames);

        // Assert
        Assert.NotNull(game);
        Assert.NotEqual(Guid.Empty, game.Id);
        Assert.Equal(6, game.Players.Count);

        var fetchedGame = await gameService.GetGameByIdAsync(game.Id);
        Assert.NotNull(fetchedGame);
        Assert.Equal(6, fetchedGame.Players.Count);
    }

    [Fact]
    public async Task GetPaginatedGamesAsync_ReturnsPagedResults()
    {
        // Arrange
        using var dbContext = CreateDbContext();
        var deckService = new DeckService();
        var scoringEngine = new ScoringEngineService();
        var gameService = new GameService(dbContext, deckService, scoringEngine);

        var playerNames = new List<string> { "Alice", "Bob", "Carol", "Dave", "Eve", "Frank" };
        await gameService.CreateGameAsync(playerNames);
        await gameService.CreateGameAsync(playerNames);

        // Act
        var result = await gameService.GetPaginatedGamesAsync(1, 10);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.Items.Count);
    }

    [Fact]
    public async Task ReDealGameAsync_UpdatesGameWithNewCards()
    {
        // Arrange
        using var dbContext = CreateDbContext();
        var deckService = new DeckService();
        var scoringEngine = new ScoringEngineService();
        var gameService = new GameService(dbContext, deckService, scoringEngine);

        var playerNames = new List<string> { "Alice", "Bob", "Carol", "Dave", "Eve", "Frank" };
        var createdGame = await gameService.CreateGameAsync(playerNames);

        // Act
        var redealtGame = await gameService.ReDealGameAsync(createdGame.Id);

        // Assert
        Assert.NotNull(redealtGame);
        Assert.Equal(createdGame.Id, redealtGame.Id);
        Assert.Equal(6, redealtGame.Players.Count);
    }
}
