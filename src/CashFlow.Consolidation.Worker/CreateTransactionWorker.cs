using CashFlow.Consolidation.Application;
using CashFlow.Consolidation.Application.Responses;
using CashFlow.Lib.EventBus;

namespace CashFlow.Consolidation.Worker;

public class CreateTransactionWorker(
    ILogger<CreateTransactionWorker> logger,
    IEventBus eventBus,
    IServiceScopeFactory scopeFactory)
    : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        const string queueName = "transaction.created";
        
        await eventBus.SubscribeAsync<TransactionCreated>(queueName, async dto =>
        {
            try
            {
                await using var scope = scopeFactory.CreateAsyncScope();
                var service = scope.ServiceProvider.GetRequiredService<ICreateTransaction>();

                await service.ExecuteAsync(dto);
                
                logger.LogInformation("Transaction {TransactionId} processed successfully", dto.Id);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing transaction {TransactionId}", dto.Id);
                throw;
            }
        });
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}