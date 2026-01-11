using CashFlow.Domain.Entities;
using CashFlow.Domain.Repositories;
using MongoDB.Driver;

namespace CashFlow.Data.Repositories;

public class ConsolidationRepository : IConsolidationRepository
{
    private readonly CashFlowMongoContext _context;

    public ConsolidationRepository(CashFlowMongoContext context)
    {
        _context = context;
    }

    public async Task UpsertAsync(ConsolidationTransaction transaction, CancellationToken token)
    {
        var filter = Builders<ConsolidationTransaction>.Filter.Eq(t => t.Id, transaction.Id);
        await _context.ConsolidationTransactions.ReplaceOneAsync(
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

    public async Task<List<ConsolidationTransaction>> GetTransactionsByDailyClosureIdAsync(Guid dailyClosureId, CancellationToken token)
    {
        var filter = Builders<ConsolidationTransaction>.Filter.Eq(t => t.DailyClosureId, dailyClosureId);
        var transactions = await _context.ConsolidationTransactions
            .Find(filter)
            .ToListAsync(token);

        return transactions;
    }
}
