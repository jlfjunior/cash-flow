using CashFlow.Domain;
using CashFlow.Domain.Events;
using CashFlow.Features.CreateAccount;
using CashFlow.Features.Transactions.Requests;
using CashFlow.Lib.EventBus;

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
        
        await eventBus.SubscribeAsync<CustomerCreated>(queueName, async customerCreated =>
        {
            try
            {
                await using var scope = scopeFactory.CreateAsyncScope();
                var command = scope.ServiceProvider.GetRequiredService<ICommand<CreateAccountRequest>>();

                await command.ExecuteAsync(new CreateAccountRequest(customerCreated.Id), cancellationToken);
                
                logger.LogInformation("Account created. CustomerId {Id}", customerCreated.Id);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing customer created event. Id {CustomerId}", customerCreated.Id);
                throw;
            }
        });
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}

