using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MovieTracker.Api.Data;

public class AppDbContext : IdentityDbContext<IdentityUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<WatchlistItem> Watchlist => Set<WatchlistItem>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // A user can't add the same title to their list twice
        builder.Entity<WatchlistItem>()
            .HasIndex(w => new { w.UserId, w.TmdbId, w.MediaType })
            .IsUnique();
    }
}