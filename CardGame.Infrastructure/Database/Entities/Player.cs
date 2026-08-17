using System.Text.Json.Serialization;

namespace CardGame.Infrastructure.Database.Entities;

public class Player : Base
{
    public string Name { get; set; } = null!;
    public Guid GameId { get; set; }
    public int SeatNumber { get; set; } 

    [JsonIgnore]
    public Game? Game { get; set; }

    public ICollection<Card> Cards { get; set; } = [];
    public Score? Score { get; set; }

}
