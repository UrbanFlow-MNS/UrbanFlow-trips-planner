
using System.Text.Json.Serialization;
using UrbanFlow_trips_planner.API.GrpcServices;
using UrbanFlow_trips_planner.Domain.Interfaces;
using UrbanFlow_trips_planner.Domain.Services;
using UrbanFlow_trips_planner.Infrastructure.Providers;
using UrbanFlow_trips;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthorization();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = false;
    });builder.Services.AddGrpc();



builder.Services.AddGrpcClient<Tripper.TripperClient>(options =>
{
    options.Address = new Uri(builder.Configuration["Grpc:TripperUrl"]!);
});

builder.Services.AddScoped<IRouteProvider, RouteProvider>();
builder.Services.AddScoped<IPathfinderService, PathfinderService>();
builder.Services.AddScoped<IWalkingRoutingService, OsrmRoutingService>();
builder.Services.AddSingleton<PrometheusService>();
builder.Services.AddControllers();

var app = builder.Build();

app.UseAuthorization();
app.MapControllers();

app.Run();

//namespace UrbanFlow_trips_planner;