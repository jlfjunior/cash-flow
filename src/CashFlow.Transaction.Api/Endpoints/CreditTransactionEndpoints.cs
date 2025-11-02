using CashFlow.Transaction.Api.Application;
using CashFlow.Transaction.Api.Application.Requests;
using CashFlow.Transaction.Api.Domain.Repositories;

namespace CashFlow.Transaction.Api.Endpoints;

public static class CreditTransactionEndpoints
{
    public static void MapTransactionEndpoints(this WebApplication app)
    {
        app.MapGet("/transactions", async (IRepository repository) =>
            {
                var transactions = await repository.SearchAsync();

                return Results.Ok(transactions);
            })
            .WithSummary("Get all transactions")
            .WithDescription(
                "Retrieves a complete list of all financial transactions in the system, including both credit and debit entries.")
            .WithTags("Transactions")
            .WithName("GetTransactions");

        app.MapPost("/transactions/credit",
                async (CreateTransactionRequest request, ICreateTransaction transactionService) =>
                {
                    var transaction = await transactionService.ExecuteAsync(request);

                    return Results.Ok(transaction);
                })
            .WithSummary("Add credit transaction")
            .WithDescription("Creates a new credit or debit transaction into an account.")
            .WithTags("Transactions")
            .WithName("AddTransaction");
    }
}
