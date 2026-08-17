namespace CardGame.Infrastructure.Database.Entities;

public class Game : Base
{
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime LastUpdatedAtUtc { get; private set; } = DateTime.UtcNow;
    public ICollection<Player> Players { get; set; } = [];

}
