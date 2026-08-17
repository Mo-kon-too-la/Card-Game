using CardGame.Infrastructure.Database.Entities;
using CardGame.Infrastructure.Services;
using CardGame.Infrastructure.Shared;
using CardGame.Server.DTOs;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace CardGame.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GamesController : ControllerBase
{
    private readonly IGameService _gameService;
    private readonly ILogger<GamesController> _logger;

    public GamesController(IGameService gameService, ILogger<GamesController> logger)
    {
        _gameService = gameService;
        _logger = logger;
    }

    [EndpointDescription("Starts a new game, deals 5 cards each to 6 players from two 52-card decks, calculates scores/winners, and persists the result.")]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(HttpValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [HttpPost]
    public async Task<Results<CreatedAtRoute<GameDto>, ValidationProblem>> CreateGame(List<string> players)
    {
        try
        {
            if (players == null || players.Count != 6)
            {
                var validationErrors = new Dictionary<string, string[]>
                {
                    { "Players", new[] { "A total of 6 players are required to start a game." } }
                };
                return TypedResults.ValidationProblem(validationErrors);
            }

            var game = await _gameService.CreateGameAsync(players);
            return TypedResults.CreatedAtRoute(game.ToDto(), nameof(GetGameById), new { game.Id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create new game.");
            throw new Exception($"An error occurred while creating and dealing the new game: {ex.Message}");
        }
    }

    [EndpointDescription("Re-deals cards for an existing game, recalculates scores and tie-breakers, and updates persistent state.")]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    [ProducesResponseType(typeof(GameDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [HttpPost("{id:guid}/redeal")]
    public async Task<Results<Ok<GameDto>, NotFound<string>>> RedealGame(Guid id)
    {
        try
        {
            var updatedGame = await _gameService.ReDealGameAsync(id);
            if (updatedGame == null)
            {
                return TypedResults.NotFound($"Game with ID '{id}' was not found.");
            }

            return TypedResults.Ok(updatedGame.ToDto());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to re-deal game {GameId}", id);
            throw new Exception($"An error occurred while re-dealing game '{id}': {ex.Message}");
        }
    }

    [EndpointDescription("Fetches details for a specific game by ID.")]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    [ProducesResponseType(typeof(GameDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [HttpGet("{id:guid}", Name = "GetGameById")]
    public async Task<Results<Ok<GameDto>, NotFound<string>>> GetGameById(Guid id)
    {
        try
        {
            var game = await _gameService.GetGameByIdAsync(id);
            if (game == null)
            {
                return TypedResults.NotFound($"Game with ID '{id}' was not found.");
            }

            return TypedResults.Ok(game.ToDto());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve game {GameId}", id);
            throw new Exception($"An error occurred while retrieving game '{id}': {ex.Message}");
        }
    }

    [EndpointDescription("Fetches recent games with pagination, sorting, and filtering.")]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    [ProducesResponseType(typeof(PagedResults<GameDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(HttpValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [HttpGet]
    public async Task<Results<Ok<PagedResults<GameDto>>, ValidationProblem>> GetGames(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? sortBy = "date",
        [FromQuery] string? sortDirection = "desc",
        [FromQuery] string? filterPlayerName = null)
    {
        try
        {
            if (page < 1)
            {
                var validationErrors = new Dictionary<string, string[]>
                {
                    { "PageNumber", new[] { "Page number must be 1 or greater." } }
                };

                return TypedResults.ValidationProblem(validationErrors);
            }

            var result = await _gameService.GetPaginatedGamesAsync(
                page,
                pageSize,
                sortBy,
                sortDirection,
                filterPlayerName);
            return TypedResults.Ok(result.ToDto());
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch paginated games.");
            throw new Exception($"An error occurred while retrieving past games: {ex.Message}");
        }
    }
}

