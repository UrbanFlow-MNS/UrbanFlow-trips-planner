using Grpc.Core;
using UrbanFlow_trips_planner.GrpcService;

namespace UrbanFlow_trips_planner.API.GrpcServices;

public class GreeterService : Greeter.GreeterClient
{
    private readonly ILogger<GreeterService> _logger;

    public GreeterService(ILogger<GreeterService> logger)
    {
        _logger = logger;
    }

    public Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
    {
        return Task.FromResult(new HelloReply
        {
            Message = "Hello " + request.Name
        });
    }
}

