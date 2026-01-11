using CashFlow.Domain.Entities;
using CashFlow.Domain.Repositories;
using MongoDB.Driver;

namespace CashFlow.Data.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly CashFlowMongoContext _context;

    public CustomerRepository(CashFlowMongoContext context)
    {
        _context = context;
    }

    public async Task UpsertAsync(Customer customer, CancellationToken token)
    {
        var filter = Builders<Customer>.Filter.Eq(c => c.Id, customer.Id);
        await _context.Customers.ReplaceOneAsync(
            filter, 
            customer, 
            new ReplaceOptions { IsUpsert = true }, 
            token);
    }

    public async Task UpsertAsync(IEnumerable<OutboxMessage> events, CancellationToken token)
    {
        if (events == null || !events.Any())
            return;

        await _context.OutboxMessages.InsertManyAsync(events, cancellationToken: token);
    }

    public async Task CommitAsync(CancellationToken token)
    {
        // MongoDB operations are atomic by default, so this is a no-op for simple operations
        // For multi-document transactions, we would use IClientSessionHandle
        await Task.CompletedTask;
    }

    public async Task<Customer> GetByIdAsync(Guid id)
    {
        var filter = Builders<Customer>.Filter.Eq(c => c.Id, id);
        var customer = await _context.Customers
            .Find(filter)
            .FirstOrDefaultAsync();

        return customer;
    }

    public async Task<IEnumerable<Customer>> SearchAsync()
    {
        var customers = await _context.Customers
            .Find(FilterDefinition<Customer>.Empty)
            .ToListAsync();

        return customers;
    }

    public async Task<OutboxMessage?> GetPendingOutboxMessageByTypeAsync(string type, CancellationToken token)
    {
        var filter = Builders<OutboxMessage>.Filter.And(
            Builders<OutboxMessage>.Filter.Eq(o => o.Status, OutboxStatus.Pending),
            Builders<OutboxMessage>.Filter.Eq(o => o.Type, type)
        );
        
        var message = await _context.OutboxMessages
            .Find(filter)
            .FirstOrDefaultAsync(token);

        return message;
    }

    public async Task UpdateOutboxMessageAsync(OutboxMessage message, CancellationToken token)
    {
        var filter = Builders<OutboxMessage>.Filter.Eq(o => o.Id, message.Id);
        await _context.OutboxMessages.ReplaceOneAsync(filter, message, cancellationToken: token);
    }
}
