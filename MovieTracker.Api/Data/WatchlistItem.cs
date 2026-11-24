namespace MovieTracker.Api.Data;

public enum WatchStatus { Planned, Watching, Completed }

public class WatchlistItem
{
    public int Id { get; set; }
    public string UserId { get; set; } = "";
    public int TmdbId { get; set; }
    public string MediaType { get; set; } = "movie"; // "movie" or "tv"
    public WatchStatus Status { get; set; } = WatchStatus.Planned;
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
}