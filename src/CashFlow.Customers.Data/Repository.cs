using CashFlow.Customers.Domain.Repositories;
using CashFlow.Customers.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CashFlow.Customers.Data;

public class Repository : IRepository
{
    private readonly CustomerContext _context;
    
    public Repository(CustomerContext context) => _context = context;

    public async Task UpsertAsync(Customer customer, CancellationToken token)
    {
        
        
    }

    public async Task<Customer> GetByIdAsync(Guid id)
    {
        var customer = await _context.Customers
            .FirstOrDefaultAsync(x => x.Id == id);

        return customer;
    }

    public async Task<IEnumerable<Customer>> SearchAsync()
    {
        var customers = await _context.Customers
            .ToListAsync();

        return customers;
    }
}