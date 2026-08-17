using CardGame.Infrastructure.Database.Entities;

namespace CardGame.Infrastructure.Services;

public class ScoringEngineService : IScoringEngine
{
    public void CalculateScoresAndWinners(ICollection<Player> players)
    {
        if (players == null || players.Count == 0) return;

        foreach (var player in players)
        {
            int handSum = player.Cards.Sum(c => c.Value);
            long suitProduct = player.Cards.Aggregate(1L, (acc, c) => acc * c.SuitValue);

            player.Score = new Score
            {
                PlayerId = player.Id,
                HandSum = handSum,
                SuitProduct = suitProduct,
                IsTiedForHighestHand = false,
                IsWinner = false
            };
        }

        int maxHandSum = players.Max(p => p.Score!.HandSum);
        var topPlayers = players.Where(p => p.Score!.HandSum == maxHandSum).ToList();

        if (topPlayers.Count == 1)
        {
            topPlayers[0].Score!.IsWinner = true;
            return;
        }

        foreach (var p in topPlayers)
        {
            p.Score!.IsTiedForHighestHand = true;
        }

        long maxSuitProduct = topPlayers.Max(p => p.Score!.SuitProduct);

        foreach (var winner in topPlayers.Where(p => p.Score!.SuitProduct == maxSuitProduct))
        {
            winner.Score!.IsWinner = true;
        }
    }
}
