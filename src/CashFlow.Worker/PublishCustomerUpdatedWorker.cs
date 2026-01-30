using CashFlow.Features.Customers;
using CashFlow.Features.UpdateCustomer;

namespace CashFlow.Worker;

public class PublishCustomerUpdatedWorker(ILogger<PublishCustomerUpdatedWorker> logger, IServiceScopeFactory scopeFactory) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            
            await using var scope = scopeFactory.CreateAsyncScope();
            var publishCustomerUpdated = scope.ServiceProvider.GetRequiredService<IPublishCustomerUpdated>();
            await publishCustomerUpdated.ExecuteAsync(stoppingToken);

            await Task.Delay(10000, stoppingToken);
        }
    }
}