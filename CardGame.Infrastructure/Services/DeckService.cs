using System.Security.Cryptography;
using CardGame.Infrastructure.Database.Entities;

namespace CardGame.Infrastructure.Services;

public class DeckService : IDeckService
{
    public List<Player> DealCards(List<string> playerNames)
    {
        var pool = Card.GetCards();

        for (int idx = pool.Count - 1; idx > 0; idx--)
        {
            int key = RandomNumberGenerator.GetInt32(idx + 1);
            (pool[idx], pool[key]) = (pool[key], pool[idx]);
        }

        var players = new List<Player>();
        int cardIndex = 0;

        for (int p = 1; p <= 6; p++)
        {
            string name = (playerNames != null && p - 1 < playerNames.Count && !string.IsNullOrWhiteSpace(playerNames[p - 1]))
                ? playerNames[p - 1]
                : $"Player {p}";

            var player = new Player
            {
                SeatNumber = p,
                Name = name,
                Cards = []
            };

            for (int c = 0; c < 5; c++)
            {
                var card = pool[cardIndex++];
                card.PlayerId = player.Id;
                player.Cards.Add(card);
            }

            players.Add(player);
        }

        return players;
    }
}

