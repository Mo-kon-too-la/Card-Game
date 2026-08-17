using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CardGame.Infrastructure.Database;
using CardGame.Infrastructure.Services;
using CardGame.Server.Controllers;
using CardGame.Server.DTOs;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace CardGame.Tests.IntegrationTests;

public class GamesControllerTests
{
    private (GamesController Controller, CardGameDbContext DbContext) CreateController()
    {
        var options = new DbContextOptionsBuilder<CardGameDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var dbContext = new CardGameDbContext(options);
        var deckService = new DeckService();
        var scoringEngine = new ScoringEngineService();
        var gameService = new GameService(dbContext, deckService, scoringEngine);
        var logger = NullLogger<GamesController>.Instance;
        var controller = new GamesController(gameService, logger);
        return (controller, dbContext);
    }

    [Fact]
    public async Task CreateGame_ValidPlayers_ReturnsCreatedAtRouteResult()
    {
        // Arrange
        var (controller, _) = CreateController();
        var players = new List<string> { "Alice", "Bob", "Carol", "Dave", "Eve", "Frank" };

        // Act
        var result = await controller.CreateGame(players);

        // Assert
        Assert.IsType<CreatedAtRoute<GameDto>>(result.Result);
        var createdAtRoute = (CreatedAtRoute<GameDto>)result.Result;
        Assert.NotNull(createdAtRoute.RouteValues);
        Assert.True(createdAtRoute.RouteValues.ContainsKey("Id"));
    }

    [Fact]
    public async Task CreateGame_InvalidPlayerCount_ReturnsValidationProblem()
    {
        // Arrange
        var (controller, _) = CreateController();
        var players = new List<string> { "Alice", "Bob" }; // Invalid player count

        // Act
        var result = await controller.CreateGame(players);

        // Assert
        Assert.IsType<ValidationProblem>(result.Result);
    }

    [Fact]
    public async Task GetGameById_ExistingId_ReturnsGameDto()
    {
        // Arrange
        var (controller, _) = CreateController();
        var players = new List<string> { "Alice", "Bob", "Carol", "Dave", "Eve", "Frank" };
        var createResult = await controller.CreateGame(players);
        var createdAtRoute = (CreatedAtRoute<GameDto>)createResult.Result;
        var gameId = (Guid)createdAtRoute.RouteValues!["Id"]!;

        // Act
        var getResult = await controller.GetGameById(gameId);

        // Assert
        Assert.IsType<Ok<GameDto>>(getResult.Result);
        var okResult = (Ok<GameDto>)getResult.Result;
        Assert.NotNull(okResult.Value);
        Assert.Equal(gameId, okResult.Value.Id);
        Assert.Equal(6, okResult.Value.Players.Count);
    }

    [Fact]
    public async Task GetGameById_NonExistentId_ReturnsNotFound()
    {
        // Arrange
        var (controller, _) = CreateController();

        // Act
        var getResult = await controller.GetGameById(Guid.NewGuid());

        // Assert
        Assert.IsType<NotFound<string>>(getResult.Result);
    }

    [Fact]
    public async Task GetGames_ReturnsPaginatedResults()
    {
        // Arrange
        var (controller, _) = CreateController();
        var players = new List<string> { "Alice", "Bob", "Carol", "Dave", "Eve", "Frank" };
        await controller.CreateGame(players);
        await controller.CreateGame(players);

        // Act
        var result = await controller.GetGames(page: 1, pageSize: 10);

        // Assert
        Assert.IsType<Ok<CardGame.Infrastructure.Shared.PagedResults<GameDto>>>(result.Result);
        var okResult = (Ok<CardGame.Infrastructure.Shared.PagedResults<GameDto>>)result.Result;
        Assert.NotNull(okResult.Value);
        Assert.Equal(2, okResult.Value.TotalCount);
        Assert.Equal(2, okResult.Value.Items.Count);
    }

    [Fact]
    public async Task RedealGame_ExistingId_ReturnsUpdatedGameDto()
    {
        // Arrange
        var (controller, _) = CreateController();
        var players = new List<string> { "Alice", "Bob", "Carol", "Dave", "Eve", "Frank" };
        var createResult = await controller.CreateGame(players);
        var createdAtRoute = (CreatedAtRoute<GameDto>)createResult.Result;
        var gameId = (Guid)createdAtRoute.RouteValues!["Id"]!;

        // Act
        var redealResult = await controller.RedealGame(gameId);

        // Assert
        Assert.IsType<Ok<GameDto>>(redealResult.Result);
        var okResult = (Ok<GameDto>)redealResult.Result;
        Assert.NotNull(okResult.Value);
        Assert.Equal(gameId, okResult.Value.Id);
        Assert.Equal(6, okResult.Value.Players.Count);
    }
}
