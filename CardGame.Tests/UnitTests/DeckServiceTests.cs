using System;
using System.Collections.Generic;
using System.Linq;
using CardGame.Infrastructure.Database.Entities;
using CardGame.Infrastructure.Services;
using Xunit;

namespace CardGame.Tests.UnitTests;

public class DeckServiceTests
{
    private readonly DeckService _deckService = new();

    [Fact]
    public void DealCards_ReturnsSixPlayers_WithFiveCardsEach()
    {
        // Arrange
        var playerNames = new List<string> { "Alice", "Bob", "Carol", "Dave", "Eve", "Frank" };

        // Act
        var players = _deckService.DealCards(playerNames);

        // Assert
        Assert.Equal(6, players.Count);
        for (int i = 0; i < 6; i++)
        {
            var p = players[i];
            Assert.Equal(i + 1, p.SeatNumber);
            Assert.Equal(playerNames[i], p.Name);
            Assert.Equal(5, p.Cards.Count);
        }
    }

    [Fact]
    public void DealCards_HandlesNullOrPartialPlayerNames_WithDefaults()
    {
        // Arrange
        var playerNames = new List<string> { "Alice", "" };

        // Act
        var players = _deckService.DealCards(playerNames);

        // Assert
        Assert.Equal(6, players.Count);
        Assert.Equal("Alice", players[0].Name);
        Assert.Equal("Player 2", players[1].Name);
        Assert.Equal("Player 6", players[5].Name);
    }

    [Fact]
    public void DealCards_DealsThirtyCardsTotalFromTwoDecks()
    {
        // Arrange
        var playerNames = new List<string> { "A", "B", "C", "D", "E", "F" };

        // Act
        var players = _deckService.DealCards(playerNames);
        var allCards = players.SelectMany(p => p.Cards).ToList();

        // Assert
        Assert.Equal(30, allCards.Count);
        Assert.All(allCards, c => Assert.True(c.DeckId == 1 || c.DeckId == 2));
        Assert.All(allCards, c => Assert.True(c.Value >= 2 && c.Value <= 13));
        Assert.All(allCards, c => Assert.True(c.SuitValue >= 1 && c.SuitValue <= 4));
    }
}
