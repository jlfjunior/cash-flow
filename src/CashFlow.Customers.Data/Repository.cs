using CashFlow.Customers.Domain.Repositories;
using CashFlow.Customers.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CashFlow.Customers.Data;

public class Repository(CustomerContext context) : IRepository
{
    public async Task UpsertAsync(Customer customer, CancellationToken token)
    {
        context.Customers.Add(customer);
        await context.SaveChangesAsync(token);
    }

    public async Task<Customer> GetByIdAsync(Guid id)
    {
        var customer = await context.Customers
            .FirstOrDefaultAsync(x => x.Id == id);

        return customer;
    }

    public async Task<IEnumerable<Customer>> SearchAsync()
    {
        var customers = await context.Customers
            .ToListAsync();

        return customers;
    }
}