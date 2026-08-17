using CardGame.Infrastructure.Database.Entities;

namespace CardGame.Infrastructure.Services;

public interface IDeckService
{
    /// <summary>
    /// Deals cards to players based on the provided player names.
    /// </summary>
    /// <param name="playerNames">The list of player names.</param>
    /// <returns>The list of players with their dealt cards.</returns>
    List<Player> DealCards(List<string> playerNames);
}
