using CashFlow.Lib.EventBus;
using CashFlow.Transaction.Application;
using CashFlow.Transaction.Application.Responses;

namespace CashFlow.Transaction.Api;

public class CustomerCreatedConsumer : IHostedService
{
    private readonly ILogger<CustomerCreatedConsumer> _logger;
    private readonly IEventBus _eventBus;
    private readonly IServiceScopeFactory _scopeFactory;


    public CustomerCreatedConsumer(ILogger<CustomerCreatedConsumer> logger, IEventBus eventBus, IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _eventBus = eventBus;
        _scopeFactory = scopeFactory;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        const string queueName = "queuing.customers.created";
        
        await _eventBus.SubscribeAsync<CustomerCreatedResponse>(queueName, async customerCreatedResponse =>
        {
            try
            {
                await using var scope = _scopeFactory.CreateAsyncScope();
                var dailyClosureService = scope.ServiceProvider.GetRequiredService<ICreateAccount>();

                await dailyClosureService.ExecuteAsync(customerCreatedResponse.Id, cancellationToken);
                
                _logger.LogInformation("Account crated. CustomerId {Id}", customerCreatedResponse.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing customer created event. Id {TransactionId}", customerCreatedResponse.Id);
                throw;
            }
        });
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}