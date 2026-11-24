using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieTracker.Api.Data;

namespace MovieTracker.Api.Controllers;

public record AddWatchlistRequest(int TmdbId, string MediaType);
public record UpdateStatusRequest(WatchStatus Status);

[ApiController]
[Route("api/watchlist")]
[Authorize]
public class WatchlistController : ControllerBase
{
    private readonly AppDbContext _db;

    public WatchlistController(AppDbContext db)
    {
        _db = db;
    }

    // The logged-in user's ID, taken from their token
    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    // GET api/watchlist - list the current user's items
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var items = await _db.Watchlist
            .Where(w => w.UserId == UserId)
            .OrderByDescending(w => w.AddedAt)
            .ToListAsync();

        return Ok(items);
    }

    // POST api/watchlist - add a title to the list
    [HttpPost]
    public async Task<IActionResult> Add(AddWatchlistRequest req)
    {
        var exists = await _db.Watchlist.AnyAsync(w =>
            w.UserId == UserId &&
            w.TmdbId == req.TmdbId &&
            w.MediaType == req.MediaType);

        if (exists)
            return Conflict("Already on your list.");

        var item = new WatchlistItem
        {
            UserId = UserId,
            TmdbId = req.TmdbId,
            MediaType = req.MediaType
        };

        _db.Watchlist.Add(item);
        await _db.SaveChangesAsync();

        return Ok(item);
    }

    // PUT api/watchlist/5 - change an item's status
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateStatus(int id, UpdateStatusRequest req)
    {
        var item = await _db.Watchlist
            .FirstOrDefaultAsync(w => w.Id == id && w.UserId == UserId);

        if (item is null)
            return NotFound();

        item.Status = req.Status;
        await _db.SaveChangesAsync();

        return Ok(item);
    }

    // DELETE api/watchlist/5 - remove an item
    [HttpDelete("{id}")]
    public async Task<IActionResult> Remove(int id)
    {
        var item = await _db.Watchlist
            .FirstOrDefaultAsync(w => w.Id == id && w.UserId == UserId);

        if (item is null)
            return NotFound();

        _db.Watchlist.Remove(item);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}