using CashFlow.Consolidation.Domain.Entities;
using CashFlow.Consolidation.Domain.Repositories;
using MongoDB.Driver;

namespace CashFlow.Consolidation.Data;

public class Repository : IRepository
{
    private readonly ConsolidationMongoContext _context;

    public Repository(ConsolidationMongoContext context)
    {
        _context = context;
    }

    public async Task UpsertAsync(Transaction transaction, CancellationToken token)
    {
        var filter = Builders<Transaction>.Filter.Eq(t => t.Id, transaction.Id);
        await _context.Transactions.ReplaceOneAsync(
            filter,
            transaction,
            new ReplaceOptions { IsUpsert = true },
            token);
    }

    public async Task UpsertAsync(DailyClosure dailyClosure, CancellationToken token)
    {
        var filter = Builders<DailyClosure>.Filter.Eq(dc => dc.Id, dailyClosure.Id);
        await _context.DailyClosures.ReplaceOneAsync(
            filter,
            dailyClosure,
            new ReplaceOptions { IsUpsert = true },
            token);
    }

    public async Task<DailyClosure?> GetDailyClosureByDateAsync(DateOnly date, Guid customerId, CancellationToken token)
    {
        var filter = Builders<DailyClosure>.Filter.And(
            Builders<DailyClosure>.Filter.Eq(dc => dc.ReferenceDate, date),
            Builders<DailyClosure>.Filter.Eq(dc => dc.CustomerId, customerId)
        );
        
        var dailyClosure = await _context.DailyClosures
            .Find(filter)
            .FirstOrDefaultAsync(token);

        return dailyClosure;
    }

    public async Task<List<Transaction>> GetTransactionsByDailyClosureIdAsync(Guid dailyClosureId, CancellationToken token)
    {
        var filter = Builders<Transaction>.Filter.Eq(t => t.DailyClosureId, dailyClosureId);
        var transactions = await _context.Transactions
            .Find(filter)
            .ToListAsync(token);

        return transactions;
    }
}

