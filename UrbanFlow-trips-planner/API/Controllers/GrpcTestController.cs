using Microsoft.AspNetCore.Mvc;
using UrbanFlow_trips_planner.GrpcService;

namespace UrbanFlow_trips_planner.API.Controllers;


[Microsoft.AspNetCore.Components.Route("api/[controller]")]
[ApiController]
public class GrpcTestController
{
    [HttpGet("test-grpc")]
    public async Task<IResult> TestGrpc(Greeter.GreeterClient client)
    {
        try
        {
            var reply = await client.SayHelloAsync(new HelloRequest { Name = "Test" });
            return Results.Ok(reply.Message);
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message);
        }
    }
}
