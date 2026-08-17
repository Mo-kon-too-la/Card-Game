using CardGame.Infrastructure.Database.Entities;

namespace CardGame.Infrastructure.Services;

public interface IScoringEngine
{
    /// <summary>
    /// Evaluates hand scores, suit-product tie breakers (if tied for top hand score), and marks winner status for all players.
    /// </summary>
    void CalculateScoresAndWinners(ICollection<Player> players);
}
