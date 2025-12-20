using CashFlow.Lib.EventBus;
using CashFlow.Lib.Sharable;
using CashFlow.Transactions.Application.Requests;
using CashFlow.Transactions.Application.Responses;

namespace CashFlow.Worker;

public class CreateAccountWorker(
    ILogger<CreateAccountWorker> logger,
    IEventBus eventBus,
    IServiceScopeFactory scopeFactory)
    : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        const string queueName = "queuing.customers.created";
        
        await eventBus.SubscribeAsync<CustomerCreatedResponse>(queueName, async customerCreatedResponse =>
        {
            try
            {
                await using var scope = scopeFactory.CreateAsyncScope();
                var command = scope.ServiceProvider.GetRequiredService<ICommand<CreateAccountRequest>>();

                await command.ExecuteAsync(new CreateAccountRequest(customerCreatedResponse.Id), cancellationToken);
                
                logger.LogInformation("Account crated. CustomerId {Id}", customerCreatedResponse.Id);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing customer created event. Id {TransactionId}", customerCreatedResponse.Id);
                throw;
            }
        });
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}

