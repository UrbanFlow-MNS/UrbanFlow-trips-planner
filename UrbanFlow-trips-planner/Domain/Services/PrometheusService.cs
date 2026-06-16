using Prometheus;
using UrbanFlow_trips_planner.Domain.Interfaces;

namespace UrbanFlow_trips_planner.Domain.Services;

public class PrometheusService
{
    public PrometheusService()
    {
        try
        {
            Metrics.DefaultRegistry.SetStaticLabels(new Dictionary<string, string>
            {
                { "app", "dotnet9-prometheus" }
            });
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
        }

    }

    public async Task<string> GetMetricsAsync()
    {
        using var stream = new MemoryStream();
        await Metrics.DefaultRegistry.CollectAndExportAsTextAsync(stream);

        stream.Position = 0;
        using var reader = new StreamReader(stream);
        return await reader.ReadToEndAsync();
    }
}