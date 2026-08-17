using System;
using System.Collections.Generic;
using CardGame.Infrastructure.Database.Entities;
using CardGame.Infrastructure.Services;
using Xunit;

namespace CardGame.Tests.UnitTests;

public class ScoringEngineServiceTests
{
    private readonly ScoringEngineService _scoringEngine = new();

    [Fact]
    public void CalculateScoresAndWinners_SingleClearWinner_SetsIsWinnerTrue()
    {
        // Arrange
        var p1 = new Player
        {
            Id = Guid.NewGuid(),
            Name = "P1",
            Cards = new List<Card>
            {
                new Card { Value = 13, SuitValue = 1 }, // K
                new Card { Value = 13, SuitValue = 1 }, // K
                new Card { Value = 13, SuitValue = 1 }, // K
                new Card { Value = 13, SuitValue = 1 }, // K
                new Card { Value = 11, SuitValue = 1 }  // A (11) => Total = 63
            }
        };

        var p2 = new Player
        {
            Id = Guid.NewGuid(),
            Name = "P2",
            Cards = new List<Card>
            {
                new Card { Value = 2, SuitValue = 1 },
                new Card { Value = 2, SuitValue = 1 },
                new Card { Value = 2, SuitValue = 1 },
                new Card { Value = 2, SuitValue = 1 },
                new Card { Value = 2, SuitValue = 1 }   // Total = 10
            }
        };

        var players = new List<Player> { p1, p2 };

        // Act
        _scoringEngine.CalculateScoresAndWinners(players);

        // Assert
        Assert.NotNull(p1.Score);
        Assert.NotNull(p2.Score);

        Assert.Equal(63, p1.Score.HandSum);
        Assert.True(p1.Score.IsWinner);
        Assert.False(p1.Score.IsTiedForHighestHand);

        Assert.Equal(10, p2.Score.HandSum);
        Assert.False(p2.Score.IsWinner);
    }

    [Fact]
    public void CalculateScoresAndWinners_HandSumTie_UsesSuitProductAsTieBreaker()
    {
        // Both players have HandSum = 20
        // P1 suit product = 4 * 4 * 4 * 4 * 4 = 1024 (all clubs = 4)
        // P2 suit product = 1 * 1 * 1 * 1 * 1 = 1 (all diamonds = 1)
        var p1 = new Player
        {
            Id = Guid.NewGuid(),
            Name = "P1",
            Cards = new List<Card>
            {
                new Card { Value = 4, SuitValue = 4 },
                new Card { Value = 4, SuitValue = 4 },
                new Card { Value = 4, SuitValue = 4 },
                new Card { Value = 4, SuitValue = 4 },
                new Card { Value = 4, SuitValue = 4 }
            }
        };

        var p2 = new Player
        {
            Id = Guid.NewGuid(),
            Name = "P2",
            Cards = new List<Card>
            {
                new Card { Value = 4, SuitValue = 1 },
                new Card { Value = 4, SuitValue = 1 },
                new Card { Value = 4, SuitValue = 1 },
                new Card { Value = 4, SuitValue = 1 },
                new Card { Value = 4, SuitValue = 1 }
            }
        };

        var players = new List<Player> { p1, p2 };

        // Act
        _scoringEngine.CalculateScoresAndWinners(players);

        // Assert
        Assert.True(p1.Score!.IsTiedForHighestHand);
        Assert.True(p2.Score!.IsTiedForHighestHand);

        Assert.True(p1.Score.IsWinner);
        Assert.False(p2.Score.IsWinner);
    }
}
