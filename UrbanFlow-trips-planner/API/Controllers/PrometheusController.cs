using Microsoft.AspNetCore.Mvc;
using UrbanFlow_trips_planner.Domain.Services;

namespace UrbanFlow_trips_planner.API.Controllers;

[ApiController]
[Route("metrics")]
public class PrometheusController : ControllerBase
{
    private readonly PrometheusService _prometheusService;

    public PrometheusController(PrometheusService prometheusService)
    {
        _prometheusService = prometheusService;
    }

    [HttpGet]
    public async Task<IActionResult> GetMetrics()
    {
        var metrics = await _prometheusService.GetMetricsAsync();
        
        return Content(metrics, "text/plain; version=0.0.4; charset=utf-8");
    }
}