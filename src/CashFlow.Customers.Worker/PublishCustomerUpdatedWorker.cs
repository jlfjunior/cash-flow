using CashFlow.Customers.Application;
using Microsoft.Extensions.DependencyInjection;

namespace CashFlow.Customers.Worker;

public class PublishCustomerUpdatedWorker(ILogger<PublishCustomerCreatedWorker> logger, IServiceScopeFactory scopeFactory) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            
            await using var scope = scopeFactory.CreateAsyncScope();
            var publishCustomerCreated = scope.ServiceProvider.GetRequiredService<IPublishCustomerCreated>();
            await publishCustomerCreated.ExecuteAsync(stoppingToken);

            await Task.Delay(10000, stoppingToken);
        }
    }
}