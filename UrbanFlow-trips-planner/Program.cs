using DotNetEnv;
using UrbanFlow_trips_planner.API.GrpcServices;
using UrbanFlow_trips_planner.Domain.Interfaces;
using UrbanFlow_trips_planner.Domain.Services;
using UrbanFlow_trips_planner.GrpcService;


Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddGrpc();

        
builder.Services.AddHttpClient<IWalkingRoutingService, OsrmRoutingService>();
builder.Services.AddGrpcClient<Greeter.GreeterClient>(o =>
{
    o.Address = new Uri("https://localhost:6003"); //voir launchsettings pour modifier le port
});

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
        
app.Run();

//namespace UrbanFlow_trips_planner;