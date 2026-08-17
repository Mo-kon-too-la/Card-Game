using System.Text.Json.Serialization;

namespace CardGame.Infrastructure.Database.Entities;

public class Card : Base
{
    public Guid PlayerId { get; set; }
    public string Rank { get; set; } = null!;
    public string Suit { get; set; } = null!;
    public int Value { get; set; }
    public int SuitValue { get; set; }
    public int DeckId { get; set; }

    [JsonIgnore]
    public Player? Player { get; set; }

    public Card() { }

    // Since these are constant values, we can define them as static readonly fields to avoid creating new instances every time we need them.
    [JsonIgnore]
    private static readonly (string Rank, int Value)[] Ranks = new (string, int)[]
    {
        ("2", 2), ("3", 3), ("4", 4), ("5", 5), ("6", 6),
        ("7", 7), ("8", 8), ("9", 9), ("10", 10),
        ("J", 11), ("Q", 12), ("K", 13), ("A", 11)
    };

    private static readonly (string Suit, int SuitValue)[] Suits = new (string, int)[]
    {
        ("♦", 1),
        ("♥", 2),
        ("♠", 3),
        ("♣", 4)
    };

    public static List<Card> GetCards()
    {
        var cards = new List<Card>(104);

        for (int deckId = 1; deckId <= 2; deckId++)
        {
            foreach (var (suit, suitValue) in Suits)
            foreach (var (rank, val) in Ranks)
            {
                cards.Add(new Card
                {
                    DeckId = deckId,
                    Rank = rank,
                    Suit = suit,
                    Value = val,
                    SuitValue = suitValue
                });
            }
        }

        return cards;
    }
}
