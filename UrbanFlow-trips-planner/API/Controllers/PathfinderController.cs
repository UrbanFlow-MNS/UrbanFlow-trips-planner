using System.Net;
using Microsoft.AspNetCore.Http.HttpResults;
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

    /// GET /api/pathfinder/fastest?startLat=...&startLong=...&endLat=...&endLong=...&departureTimeSeconds=...
    [HttpGet("fastest")]
    public async Task<IActionResult> GetFastest(
        [FromQuery] float startLat,
        [FromQuery] float startLong,
        [FromQuery] float endLat,
        [FromQuery] float endLong,
        [FromQuery] int   departureTimeSeconds,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var results = await _pathfinderService.GetFastestRoutes(
                startLong, startLat,
                endLong, endLat,
                departureTimeSeconds,
                _walkingRoutingService);
            
            if (results.Count <= 0)
                return NotFound("Aucun trajet trouvé pour ces paramètres.");
        

            return Ok(results);
        }
        catch (HttpRequestException ex)
        {
            if (ex.StatusCode == HttpStatusCode.NotFound)
                return NotFound("No trips were found");
            
            throw new Exception(ex.Message);
        }
    }
}