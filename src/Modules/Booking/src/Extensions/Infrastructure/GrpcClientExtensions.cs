using Booking.Configuration;
using BookingFlight;
using BookingPassenger;
using BuildingBlocks.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;
using Polly;

namespace Booking.Extensions.Infrastructure;

public static class GrpcClientExtensions
{
    public static IServiceCollection AddGrpcClients(this IServiceCollection services)
    {
        var grpcOptions = services.GetOptions<GrpcOptions>("Grpc");

        services.AddGrpcClient<FlightGrpcService.FlightGrpcServiceClient>(o =>
        {
            o.Address = new Uri(grpcOptions.FlightAddress);
        })
        .AddResilienceHandler(
            "grpc-flight-resilience",
            options =>
            {
                var timeSpan = TimeSpan.FromMinutes(1);

                options.AddRetry(
                    new HttpRetryStrategyOptions
                    {
                        MaxRetryAttempts = 3,
                    });

                options.AddCircuitBreaker(
                    new HttpCircuitBreakerStrategyOptions
                    {
                        SamplingDuration = timeSpan * 2,
                    });

                options.AddTimeout(
                    new HttpTimeoutStrategyOptions
                    {
                        Timeout = timeSpan * 3,
                    });
            });

        services.AddGrpcClient<PassengerGrpcService.PassengerGrpcServiceClient>(o =>
        {
            o.Address = new Uri(grpcOptions.PassengerAddress);
        })
        .AddResilienceHandler(
            "grpc-passenger-resilience",
            options =>
            {
                var timeSpan = TimeSpan.FromMinutes(1);

                options.AddRetry(
                    new HttpRetryStrategyOptions
                    {
                        MaxRetryAttempts = 3,
                    });

                options.AddCircuitBreaker(
                    new HttpCircuitBreakerStrategyOptions
                    {
                        SamplingDuration = timeSpan * 2,
                    });

                options.AddTimeout(
                    new HttpTimeoutStrategyOptions
                    {
                        Timeout = timeSpan * 3,
                    });
            });

        return services;
    }
}