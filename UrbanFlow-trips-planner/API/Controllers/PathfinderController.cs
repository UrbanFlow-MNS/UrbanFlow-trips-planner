using Microsoft.AspNetCore.Mvc;
using UrbanFlow_trips_planner.Domain.Interfaces;

namespace UrbanFlow_trips_planner.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PathfinderController : ControllerBase
{
    private readonly IPathfinderService    _pathfinderService;
    private readonly IWalkingRoutingService _walkingRoutingService;

    public PathfinderController(
        IPathfinderService     pathfinderService,
        IWalkingRoutingService walkingRoutingService)
    {
        _pathfinderService     = pathfinderService;
        _walkingRoutingService = walkingRoutingService;
    }

    /// GET /api/pathfinder/fastest?agencyId=1&startLat=...&startLong=...&endLat=...&endLong=...&departureTimeSeconds=...
    [HttpGet("fastest")]
    public async Task<IActionResult> GetFastest(
        [FromQuery] int   agencyId,
        [FromQuery] float startLat,
        [FromQuery] float startLong,
        [FromQuery] float endLat,
        [FromQuery] float endLong,
        [FromQuery] int   departureTimeSeconds,
        CancellationToken cancellationToken = default)
    {
        var results = await _pathfinderService.GetFastestRoutes(
            agencyId,
            startLong, startLat,
            endLong,   endLat,
            departureTimeSeconds,
            _walkingRoutingService) ?? null;

        if (results == null || results.Count <= 0)
            return NotFound("Aucun trajet trouvé pour ces paramètres.");

        return Ok(results);
    }
}