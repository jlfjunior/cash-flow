using CashFlow.Customer.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace CashFlow.Customer.Data;

public class Repository : IRepository
{
    private readonly CustomerContext _context;
    
    public Repository(CustomerContext context) => _context = context;

    public async Task UpsertAsync(Domain.Entities.Customer customer, CancellationToken token)
    {
        
        
    }

    public async Task<Domain.Entities.Customer> GetByIdAsync(Guid id)
    {
        var customer = await _context.Customers
            .FirstOrDefaultAsync(x => x.Id == id);

        return customer;
    }

    public async Task<IEnumerable<Domain.Entities.Customer>> SearchAsync()
    {
        var customers = await _context.Customers
            .ToListAsync();

        return customers;
    }
}