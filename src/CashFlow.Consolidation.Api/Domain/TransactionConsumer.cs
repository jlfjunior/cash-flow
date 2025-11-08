using CashFlow.Consolidation.Api.Application;
using CashFlow.Consolidation.Api.Domain.Services;
using CashFlow.Lib.EventBus;

namespace CashFlow.Consolidation.Api.Domain;

public class TransactionConsumer : IHostedService
{
    private readonly ILogger<TransactionConsumer> _logger;
    private readonly IEventBus _eventBus;
    private readonly IServiceScopeFactory _scopeFactory;


    public TransactionConsumer(ILogger<TransactionConsumer> logger, IEventBus eventBus, IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _eventBus = eventBus;
        _scopeFactory = scopeFactory;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        const string queueName = "transaction.created";
        
        await _eventBus.SubscribeAsync<TransactionCreated>(queueName, async dto =>
        {
            try
            {
                await using var scope = _scopeFactory.CreateAsyncScope();
                var dailyClosureService = scope.ServiceProvider.GetRequiredService<IDailyClosureService>();

                await dailyClosureService.AddTransaction(dto);
                
                _logger.LogInformation("Transaction {TransactionId} processed successfully", dto.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing transaction {TransactionId}", dto.Id);
                throw;
            }
        });
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}