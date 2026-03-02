using UrbanFlow_trips_planner.API.GrpcServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddGrpc();

var app = builder.Build();
        
app.MapGrpcService<GreeterService>();

// juste pour tester le greeter
app.MapGet("/",
    () =>
        "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");


app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
        
app.Run();

//namespace UrbanFlow_trips_planner;