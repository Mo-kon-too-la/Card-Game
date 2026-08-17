using Microsoft.EntityFrameworkCore;
using CardGame.Infrastructure.Database.Entities;

namespace CardGame.Infrastructure.Database;

public class CardGameDbContext : DbContext
{
    // Since this is a small scale application, there is no need for a Unit of Work pattern

    public DbSet<Player> Players { get; set; } = null!;
    public DbSet<Game> Games { get; set; } = null!;
    public DbSet<Score> Scores { get; set; } = null!;
    public DbSet<Card> Cards { get; set; } = null!;

    public CardGameDbContext(DbContextOptions<CardGameDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
         
        modelBuilder.Entity<Game>(entity =>
        {
            entity.HasKey(g => g.Id);
            entity.HasIndex(g => g.CreatedAtUtc);

            entity.HasMany(g => g.Players)
                  .WithOne(p => p.Game)
                  .HasForeignKey(p => p.GameId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Player>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.HasIndex(p => new { p.GameId, p.SeatNumber });

            entity.HasMany(p => p.Cards)
                  .WithOne(c => c.Player)
                  .HasForeignKey(c => c.PlayerId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(p => p.Score)
                  .WithOne(s => s.Player)
                  .HasForeignKey<Score>(s => s.PlayerId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Card>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.HasIndex(c => c.PlayerId);
        });

        modelBuilder.Entity<Score>(entity =>
        {
            entity.HasKey(s => s.Id);
        });
    }
}
