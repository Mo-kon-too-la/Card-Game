using System.Text.Json.Serialization;

namespace CardGame.Infrastructure.Database.Entities;

public class Score : Base
{
    public Guid PlayerId { get; set; }
    public int HandSum { get; set; }
    public long SuitProduct { get; set; }
    public bool IsTiedForHighestHand { get; set; }
    public bool IsWinner { get; set; }

    [JsonIgnore]
    public Player? Player { get; set; }
}
