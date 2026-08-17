namespace CardGame.Infrastructure.Database.Entities;

/// <summary>
/// Represents the base class for all entities in the database.
/// </summary>
public class Base
{
    public Guid Id { get; set; } = Guid.CreateVersion7();

}
